using CsvHelper;
using dago.CsvMaps;
using dago.Data;
using dago.Models;
using dago.Models.DTOs;
using dago.Repository;
using dago.Services.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
            List<object> linhas;
            List<LinhaErroImportacaoDTO> errosPendentes = new();

            if (_cache.TryGetValue(token, out ImportCacheData dataNovo))
            {
                linhas = dataNovo.LinhasValidas ?? new List<object>();
            }
            else if (_cache.TryGetValue(token, out (List<object> preview, List<(int Linha, string Motivo)> errosCache) dataAntigo))
            {
                linhas = dataAntigo.preview ?? new List<object>();
                errosPendentes = dataAntigo.errosCache
                    .Select(e => new LinhaErroImportacaoDTO { Linha = e.Linha, Erro = e.Motivo, Severidade = "Alerta" })
                    .ToList();
            }
            else
            {
                throw new InvalidOperationException("Pré-visualização expirada ou inexistente.");
            }

            var erros = new List<LinhaErroImportacaoDTO>();
            int gravados = 0;
            int linhaNum = 0;

            foreach (dynamic item in linhas)
            {
                linhaNum++;
                try
                {
                    var normalizer = new CtrcNormalizer(_db);
                    string numeroCtrc = item.ctrc ?? "";
                    if (string.IsNullOrWhiteSpace(numeroCtrc))
                        throw new Exception("CTRC sem número.");

                    var (cliente, cidade, estado, unidade) = await normalizer.ResolverAsync(
                        (string)item.clienteRemetente,
                        (string)item.clienteDestinatario,
                        (string)item.cidadeEntrega,
                        (string)item.ufEntrega,
                        (string)item.unidadeReceptora
                    );

                    DateTime? dataEmissao = ParseDate(item.dataEmissao);
                    DateTime? dataEntrega = ParseDate(item.dataEntregaRealizada);
                    DateTime? dataOcorrencia = ParseDate(item.dataUltimaOcorrencia);

                    decimal pesoTon = 0;
                    try { pesoTon = (decimal)(item.pesoToneladas ?? 0m); }
                    catch
                    {
                        if (decimal.TryParse(((string?)item.pesoToneladas) ?? "0", NumberStyles.Any, new CultureInfo("pt-BR"), out var p))
                            pesoTon = p;
                    }

                    int leadTimeDias = await CalcularLeadTimeDiasAsync(cliente.Id, cidade.Id);

                    DateTime? dataPrevistaEntrega = null;
                    if (dataEmissao.HasValue && leadTimeDias > 0)
                    {
                        dataPrevistaEntrega = dataEmissao.Value.AddDays(leadTimeDias);
                    }


                    var ctrc = await _db.Ctrcs.FirstOrDefaultAsync(c => c.Numero == numeroCtrc);

                    if (ctrc == null)
                    {
                        ctrc = new Ctrc
                        {
                            Numero = numeroCtrc,
                            DataEmissao = dataEmissao ?? DateTime.Now,
                            NumeroNotaFiscal = item.numeroNotaFiscal,
                            ClienteId = cliente.Id,
                            CidadeDestinoId = cidade.Id,
                            EstadoDestinoId = estado.Id,
                            UnidadeId = unidade.Id,
                            Observacao = null,
                            Peso = pesoTon,
                            LeadTimeDias = leadTimeDias,
                            DataPrevistaEntrega = dataPrevistaEntrega,
                            DataEntregaRealizada = dataEntrega,
                            NotasFiscais = item.notasFiscais,
                            Destinatario = (string)item.clienteDestinatario
                        };
                        AplicarStatusEDesvio(ctrc, dataEntrega);
                        _db.Ctrcs.Add(ctrc);
                    }
                    else
                    {
                        ctrc.NumeroNotaFiscal = item.numeroNotaFiscal;
                        ctrc.ClienteId = cliente.Id;
                        ctrc.CidadeDestinoId = cidade.Id;
                        ctrc.EstadoDestinoId = estado.Id;
                        ctrc.UnidadeId = unidade.Id;
                        ctrc.Peso = pesoTon;
                        ctrc.LeadTimeDias = leadTimeDias;
                        ctrc.DataPrevistaEntrega = dataPrevistaEntrega;
                        ctrc.DataEntregaRealizada = dataEntrega;
                        ctrc.Destinatario = (string)item.clienteDestinatario;
                        ctrc.NotasFiscais = item.notasFiscais;
                        AplicarStatusEDesvio(ctrc, dataEntrega);
                        _db.Ctrcs.Update(ctrc);
                    }

                    await _db.SaveChangesAsync();

                    await RegistrarOcorrenciaSistemaAsync(ctrc.Id, dataOcorrencia, (string?)item.descricaoUltimaOcorrencia);
                    await _db.SaveChangesAsync();
                    gravados++;
                }
                catch (Exception ex)
                {
                    erros.Add(new LinhaErroImportacaoDTO
                    {
                        Linha = linhaNum,
                        Ctrc = TryGet(item, "ctrc"),
                        Erro = ex.Message,
                        Severidade = "Critico",
                        CorrigidoAutomaticamente = false,
                        Payload = item
                    });
                }
            }

            if (_cache.TryGetValue(token, out ImportCacheData cacheData))
            {
                cacheData.LinhasComErro = erros;
                _cache.Set(token, cacheData, TimeSpan.FromHours(2));
            }

            return (gravados, erros);
        }

        // === Helpers ===

        private static string? TryGet(dynamic item, string propName)
        {
            try
            {
                var dict = item as IDictionary<string, object>;
                if (dict != null && dict.ContainsKey(propName))
                    return dict[propName]?.ToString();
                return (string?)item.GetType().GetProperty(propName)?.GetValue(item);
            }
            catch { return null; }
        }

        // =============================
        // ✅ Correção para PostgreSQL UTC
        // =============================
        private static DateTime? ParseDate(string? s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return null;

            // Tenta múltiplos formatos de data válidos no Brasil
            string[] formatos = { "dd/MM/yyyy", "dd/MM/yy", "yyyy-MM-dd", "dd-MM-yyyy" };

            if (DateTime.TryParseExact(
                s.Trim(),
                formatos,
                new CultureInfo("pt-BR"),
                DateTimeStyles.None,
                out var data))
            {
                // 🔹 Força o tipo UTC (corrige erro do PostgreSQL)
                return DateTime.SpecifyKind(data, DateTimeKind.Utc);
            }

            return null;
        }


        private async Task<DateTime?> ObterDataAgendaBaseAsync(int ctrcId)
        {
            var ag = await _db.Agendas
                .Where(a => a.CtrcId == ctrcId)
                .OrderByDescending(a => a.Data)
                .Select(a => a.Data)
                .FirstOrDefaultAsync();
            return ag == default ? null : ag;
        }

        private void AplicarStatusEDesvio(Ctrc ctrc, DateTime? dataEntrega)
        {
            DateTime basePrazo;
            var dataAgenda = ObterDataAgendaBaseAsync(ctrc.Id).GetAwaiter().GetResult();
            if (dataAgenda.HasValue)
                basePrazo = dataAgenda.Value.Date;
            else
                basePrazo = ctrc.DataEmissao.Date.AddDays(ctrc.LeadTimeDias);

            if (dataEntrega.HasValue)
            {
                var desvio = (dataEntrega.Value.Date - basePrazo).Days;
                ctrc.DesvioPrazoDias = desvio;
                ctrc.StatusEntregaId = desvio > 0 ? 3 : 1;
            }
            else
            {
                ctrc.DesvioPrazoDias = null;
                ctrc.StatusEntregaId = DateTime.UtcNow.Date > basePrazo ? 2 : 2;
            }
        }

        private async Task<int> CalcularLeadTimeDiasAsync(int clienteId, int cidadeId)
        {
            var cidade = await _db.Cidades
                .Include(c => c.Estado)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == cidadeId);

            if (cidade == null) return 0;

            var lead = await _db.LeadTimesCliente
                .AsNoTracking()
                .FirstOrDefaultAsync(l =>
                    l.ClienteId == clienteId &&
                    l.TipoRegiaoId == cidade.TipoRegiaoId &&
                    l.RegiaoEstadoId == cidade.Estado.RegiaoEstadoId);

            if (lead == null)
            {
                lead = await _db.LeadTimesCliente
                    .AsNoTracking()
                    .FirstOrDefaultAsync(l =>
                        l.ClienteId == 3573 &&
                        l.TipoRegiaoId == cidade.TipoRegiaoId &&
                        l.RegiaoEstadoId == cidade.Estado.RegiaoEstadoId);
            }

            return lead?.DiasLead ?? 0;
        }

        private async Task RegistrarOcorrenciaSistemaAsync(int ctrcId, DateTime? data, string? descricao)
        {
            if (!data.HasValue || string.IsNullOrWhiteSpace(descricao))
                return;

            var existe = await _db.OcorrenciasSistema
                .AsNoTracking()
                .AnyAsync(o => o.CtrcId == ctrcId && o.Data == data && o.Descricao == descricao);
            if (existe) return;

            var ultima = await _db.OcorrenciasSistema
                .Where(o => o.CtrcId == ctrcId)
                .OrderByDescending(o => o.Data)
                .FirstOrDefaultAsync();

            int numeroOcorrencia = (ultima?.NumeroOcorrencia ?? 0) + 1;
            int? diasDesdeAnterior = ultima != null ? (data.Value.Date - ultima.Data.Date).Days : (int?)null;

            var nova = new OcorrenciaSistema
            {
                CtrcId = ctrcId,
                NumeroOcorrencia = numeroOcorrencia,
                Data = data.Value,
                Descricao = descricao,
                DiasDesdeAnterior = diasDesdeAnterior
            };

            await _db.OcorrenciasSistema.AddAsync(nova);
        }

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
    }
}
