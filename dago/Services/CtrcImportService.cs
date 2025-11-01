using CsvHelper;
using dago.CsvMaps;
using dago.Data;
using dago.Models;
using dago.Models.DTOs;
using dago.Repository;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace dago.Services
{
    public interface ICtrcImportService
    {
        Task<(string token, List<object> preview)> PreviewAsync(Stream csvStream, string praca);
        Task<(int inseridos, List<LinhaErroImportacaoDTO> erros)> ConfirmAsync(string token);
    }

    public class CtrcImportService : ICtrcImportService
    {
        private readonly AppDbContext _db;
        private readonly ICtrcRepository _repo;
        private readonly IMemoryCache _cache;

        public CtrcImportService(AppDbContext db, ICtrcRepository repo, IMemoryCache cache)
        {
            _db = db;
            _repo = repo;
            _cache = cache;
        }

        // === 1️⃣ Leitura e prévia dos dados (Preview) ===
        public async Task<(string token, List<object> preview)> PreviewAsync(Stream csvStream, string praca)
        {
            var linhas = ReadCsv(csvStream);
            var cultura = new CultureInfo("pt-BR");
            var erros = new List<(int Linha, string Motivo)>();
            var preview = new List<object>();

            praca = praca.Trim().ToUpperInvariant();
            if (praca.Length > 3)
                praca = praca[..3];

            int linhaNum = 1;
            foreach (var row in linhas)
            {
                linhaNum++;

                try
                {
                    // 🔹 Filtros iniciais
                    if (!IsTipoValido(row.TipoDocumento))
                        continue;

                    if (!string.IsNullOrWhiteSpace(praca) &&
                        !row.PracaExpedidora.Trim().StartsWith(praca, StringComparison.OrdinalIgnoreCase))
                        continue;

                    // 🔹 Validações básicas
                    if (string.IsNullOrWhiteSpace(row.SerieNumeroCtrc))
                        throw new Exception("CTRC sem número.");
                    if (string.IsNullOrWhiteSpace(row.ClienteRemetente))
                        throw new Exception("Cliente remetente ausente.");
                    if (string.IsNullOrWhiteSpace(row.ClienteDestinatario))
                        throw new Exception("Cliente destinatário ausente.");
                    if (string.IsNullOrWhiteSpace(row.CidadeEntrega) || string.IsNullOrWhiteSpace(row.UfEntrega))
                        throw new Exception("Cidade ou UF de entrega inválida.");

                    // 🔹 Conversões
                    DateTime? emissao = DateTime.TryParse(row.DataEmissao, cultura, DateTimeStyles.None, out var e) ? e : null;
                    DateTime? ocorrencia = DateTime.TryParse(row.DataUltimaOcorrencia, cultura, DateTimeStyles.None, out var o) ? o : null;
                    DateTime? entrega = DateTime.TryParse(row.DataEntregaRealizada, cultura, DateTimeStyles.None, out var d) ? d : null;

                    // 🔹 Peso (kg → toneladas)
                    decimal? pesoTon = null;
                    if (decimal.TryParse(row.PesoCalculadoKg, NumberStyles.Any, cultura, out var pesoKg))
                        pesoTon = pesoKg / 1000;

                    // 🔹 Adiciona ao preview
                    preview.Add(new
                    {
                        ctrc = row.SerieNumeroCtrc,
                        clienteRemetente = row.ClienteRemetente,
                        clienteDestinatario = row.ClienteDestinatario,
                        cidadeEntrega = row.CidadeEntrega,
                        ufEntrega = row.UfEntrega,
                        unidadeReceptora = row.UnidadeReceptora,
                        numeroNotaFiscal = row.NumeroNotaFiscal,
                        dataEmissao = emissao?.ToString("dd/MM/yyyy"),
                        dataUltimaOcorrencia = ocorrencia?.ToString("dd/MM/yyyy"),
                        descricaoUltimaOcorrencia = row.DescricaoUltimaOcorrencia,
                        dataEntregaRealizada = entrega?.ToString("dd/MM/yyyy"),
                        notasFiscais = row.NotasFiscais,
                        pesoToneladas = pesoTon
                    });
                }
                catch (Exception ex)
                {
                    erros.Add((linhaNum, ex.Message));
                }
            }

            // 🔹 Gera token e armazena em cache
            var token = Guid.NewGuid().ToString("N");
            _cache.Set(token, (preview, erros), TimeSpan.FromMinutes(30));

            return (token, preview);
        }

        // === 2️⃣ Confirmação e gravação ===
        public async Task<(int inseridos, List<LinhaErroImportacaoDTO> erros)> ConfirmAsync(string token)
        {
            if (!_cache.TryGetValue(token, out (List<object> preview, List<(int Linha, string Motivo)> errosCache) data))
                throw new InvalidOperationException("Pré-visualização expirada ou inexistente.");

            var erros = new List<LinhaErroImportacaoDTO>();
            int inseridos = 0;

            // 🔹 Recupera dados básicos do banco
            var estados = await _db.Estados.AsNoTracking().ToListAsync();
            var clientes = await _db.Clientes.AsNoTracking().ToListAsync();
            var cidades = await _db.Cidades.AsNoTracking().ToListAsync();
            var unidades = await _db.Unidades.AsNoTracking().ToListAsync();

            int linha = 0;
            foreach (dynamic item in data.preview)
            {
                linha++;
                try
                {
                    string numeroCtrc = item.ctrc ?? "";

                    if (await _repo.CtrcExisteAsync(numeroCtrc))
                        throw new Exception($"CTRC '{numeroCtrc}' já existente.");

                    var ctrc = new Ctrc
                    {
                        Numero = numeroCtrc,
                        DataEmissao = ParseDate(item.dataEmissao),
                        NumeroNotaFiscal = item.numeroNotaFiscal,
                        Observacao = item.descricaoUltimaOcorrencia ?? "",
                        Peso = (decimal)(item.pesoToneladas ?? 0)
                    };

                    await _repo.AddCtrcAsync(ctrc);
                    inseridos++;
                }
                catch (Exception ex)
                {
                    erros.Add(new LinhaErroImportacaoDTO
                    {
                        Linha = linha,
                        Ctrc = (string)item.ctrc,
                        Erro = ex.Message
                    });
                }
            }

            await _repo.SaveChangesAsync();
            _cache.Remove(token);

            return (inseridos, erros);
        }

        // === Helpers ===
        private static bool IsTipoValido(string tipo)
        {
            if (string.IsNullOrWhiteSpace(tipo))
                return false;
            tipo = tipo.Trim().ToUpperInvariant();
            return tipo == "NORMAL" || tipo == "CARGA FECHADA";
        }

        private static List<CtrcCsvRow> ReadCsv(Stream stream)
        {
            using var reader = new StreamReader(stream);

            var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HeaderValidated = null,
                MissingFieldFound = null,
                BadDataFound = null,
                TrimOptions = CsvHelper.Configuration.TrimOptions.Trim,
                DetectColumnCountChanges = true
            };

            using var csv = new CsvReader(reader, config);
            csv.Context.RegisterClassMap<CtrcCsvRowMap>();
            return csv.GetRecords<CtrcCsvRow>().ToList();
        }

        private static DateTime ParseDate(string s)
        {
            if (DateTime.TryParseExact(s?.Trim(), "dd/MM/yyyy", new CultureInfo("pt-BR"), DateTimeStyles.None, out var d))
                return d;
            return DateTime.Now;
        }
    }
}
