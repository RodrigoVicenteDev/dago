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

        // =============================
        // 1️⃣ Lê o CSV e retorna prévia
        // =============================
        public async Task<(string token, List<object> preview)> PreviewAsync(Stream csvStream, string praca)
        {
            var linhas = ReadCsv(csvStream);
            var cultura = new CultureInfo("pt-BR");
            var preview = new List<object>();

            praca = praca.Trim().ToUpperInvariant();
            if (praca.Length > 3)
                praca = praca[..3];

            foreach (var row in linhas)
            {
                try
                {
                    if (!IsTipoValido(row.TipoDocumento))
                        continue;

                    if (!string.IsNullOrWhiteSpace(praca) &&
                        !row.PracaExpedidora.Trim().StartsWith(praca, StringComparison.OrdinalIgnoreCase))
                        continue;

                    DateTime? emissao = ParseDate(row.DataEmissao);
                    DateTime? entrega = ParseDate(row.DataEntregaRealizada);
                    DateTime? ocorrencia = ParseDate(row.DataUltimaOcorrencia);

                    decimal? pesoTon = null;
                    if (decimal.TryParse(row.PesoCalculadoKg, NumberStyles.Any, cultura, out var pesoKg))
                        pesoTon = pesoKg / 1000;

                    preview.Add(new
                    {
                        ctrc = row.SerieNumeroCtrc,
                        clienteRemetente = row.ClienteRemetente,
                        cnpjRemetente = row.CnpjRemetente,
                        clienteDestinatario = row.ClienteDestinatario,
                        cidadeEntrega = row.CidadeEntrega,
                        ufEntrega = row.UfEntrega,
                        unidadeReceptora = row.UnidadeReceptora,
                        numeroNotaFiscal = row.NumeroNotaFiscal,
                        dataEmissao = emissao?.ToString("dd/MM/yyyy"),
                        dataEntregaRealizada = entrega?.ToString("dd/MM/yyyy"),
                        dataUltimaOcorrencia = ocorrencia?.ToString("dd/MM/yyyy"),
                        descricaoUltimaOcorrencia = row.DescricaoUltimaOcorrencia,
                        pesoToneladas = pesoTon,
                        notasFiscais = row.NotasFiscais
                    });
                }
                catch { /* ignora linha inválida na prévia */ }
            }

            var token = Guid.NewGuid().ToString("N");
            _cache.Set(token, preview, TimeSpan.FromMinutes(30));

            return (token, preview);
        }

        // =====================================
        // 2️⃣ Confirma e grava os dados no banco
        // =====================================
        public async Task<(int inseridos, List<LinhaErroImportacaoDTO> erros)> ConfirmAsync(string token)
        {
            if (!_cache.TryGetValue(token, out List<object>? linhas) || linhas == null)
                throw new InvalidOperationException("Pré-visualização expirada ou inexistente.");

            int gravados = 0;
            var erros = new List<LinhaErroImportacaoDTO>();
            int linhaNum = 0;

            foreach (dynamic item in linhas)
            {
                linhaNum++;
                try
                {
                    string numeroCtrc = item.ctrc ?? "";
                    if (string.IsNullOrWhiteSpace(numeroCtrc))
                        throw new Exception("CTRC sem número.");

                    var normalizer = new CtrcNormalizer(_db);
                    var (cliente, cidade, estado, unidade) = await normalizer.ResolverAsync(
                        (string)item.clienteRemetente,
                        (string)item.cnpjRemetente,   // <-- NOVO
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
                    catch { pesoTon = 0; }

                    int leadTimeDias = await CalcularLeadTimeDiasAsync(cliente.Id, cidade.Id, estado.Id);
                    DateTime? dataPrevistaEntrega = null;
                    if (dataEmissao.HasValue && leadTimeDias > 0)
                    {
                        dataPrevistaEntrega = AdicionarDiasUteis(dataEmissao.Value, leadTimeDias);
                    }



                    var ctrc = await _db.Ctrcs.FirstOrDefaultAsync(c => c.Numero == numeroCtrc);
                    if (ctrc == null)
                    {
                        ctrc = new Ctrc
                        {
                            Numero = numeroCtrc,
                            ClienteId = cliente.Id,
                            CidadeDestinoId = cidade.Id,
                            EstadoDestinoId = estado.Id,
                            UnidadeId = unidade.Id,
                            NumeroNotaFiscal = item.numeroNotaFiscal,
                            DataEmissao = dataEmissao ?? DateTime.Today,
                            DataPrevistaEntrega = dataPrevistaEntrega,
                            DataEntregaRealizada = dataEntrega,
                            Peso = pesoTon,
                            LeadTimeDias = leadTimeDias,
                            NotasFiscais = item.notasFiscais,
                            Destinatario = (string)item.clienteDestinatario
                        };
                        AplicarStatusEDesvio(ctrc, dataEntrega);
                        _db.Ctrcs.Add(ctrc);
                    }
                    else
                    {
                        ctrc.DataEmissao = dataEmissao ?? ctrc.DataEmissao;
                        ctrc.DataPrevistaEntrega = dataPrevistaEntrega;

                        if (dataEntrega.HasValue)
                            ctrc.DataEntregaRealizada = dataEntrega;

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
                    Console.WriteLine("❌ ERRO NA IMPORTAÇÃO DO CTRC");
                    Console.WriteLine($"   ➤ Linha: {linhaNum}");
                    Console.WriteLine($"   ➤ Conteúdo: {System.Text.Json.JsonSerializer.Serialize(item)}");
                    Console.WriteLine($"   ➤ Erro: {ex.Message}");
                    Console.WriteLine($"   ➤ StackTrace: {ex.StackTrace}");

                    erros.Add(new LinhaErroImportacaoDTO
                    {
                        Linha = linhaNum,
                        Erro = ex.Message,
                        Severidade = "Crítico"
                    });
                }
            }

            return (gravados, erros);
        }

        // ===============================
        // Helpers e padronização de datas
        // ===============================
        private static bool IsTipoValido(string tipo)
        {
            if (string.IsNullOrWhiteSpace(tipo)) return false;
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
                BadDataFound = null
            };
            using var csv = new CsvReader(reader, config);
            csv.Context.RegisterClassMap<CtrcCsvRowMap>();
            return csv.GetRecords<CtrcCsvRow>().ToList();
        }

        // ✅ Correção definitiva: data pura, sem UTC
        private static DateTime? ParseDate(string? s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return null;

            string[] formatos = { "dd/MM/yyyy", "dd/MM/yy", "yyyy-MM-dd", "dd-MM-yyyy" };

            if (DateTime.TryParseExact(
                s.Trim(),
                formatos,
                new CultureInfo("pt-BR"),
                DateTimeStyles.None,
                out var data))
            {
                return DateTime.SpecifyKind(data.Date, DateTimeKind.Unspecified);
            }

            return null;
        }

        private void AplicarStatusEDesvio(Ctrc ctrc, DateTime? dataEntrega)
        {
            var hoje = DateTime.Today;

            // 🔹 Define o status conforme a situação atual
            if (ctrc.DataPrevistaEntrega.HasValue)
            {
                if (ctrc.DataEntregaRealizada.HasValue)
                {
                    // ENTREGUE
                    if (ctrc.DataEntregaRealizada.Value <= ctrc.DataPrevistaEntrega.Value)
                        ctrc.StatusEntregaId = 1; // ENTREGUE NO PRAZO
                    else
                        ctrc.StatusEntregaId = 3; // ENTREGUE COM ATRASO
                }
                else
                {
                    // NÃO ENTREGUE
                    if (hoje > ctrc.DataPrevistaEntrega.Value)
                        ctrc.StatusEntregaId = 2; // ATRASADA
                    else
                        ctrc.StatusEntregaId = 7; // NO PRAZO
                }
            }
            else
            {
                // SEM DATA PREVISTA DEFINIDA
                ctrc.StatusEntregaId = 9; // PENDENTE / INDEFINIDO
            }

        }

        private static DateTime AdicionarDiasUteis(DateTime dataInicial, int diasUteis)
        {
            var data = dataInicial.Date;
            int adicionados = 0;

            while (adicionados < diasUteis)
            {
                data = data.AddDays(1);

                // pula sábado e domingo
                if (data.DayOfWeek != DayOfWeek.Saturday &&
                    data.DayOfWeek != DayOfWeek.Sunday)
                {
                    adicionados++;
                }
            }

            return data;
        }

        private async Task<int> CalcularLeadTimeDiasAsync(int clienteId, int cidadeId, int estadoId)
        {
            // 1️⃣ LeadTime específico do cliente para a combinação Cidade + Estado
            var leadCliente = await _db.LeadTimesCliente
                .AsNoTracking()
                .FirstOrDefaultAsync(l =>
                    l.ClienteId == clienteId &&
                    l.CidadeId == cidadeId &&
                    l.EstadoId == estadoId);

            if (leadCliente != null)
                return leadCliente.DiasLead;

            // 2️⃣ LeadTime do cliente "Esporádico" (ID 3573)
            var leadEsporadico = await _db.LeadTimesCliente
                .AsNoTracking()
                .FirstOrDefaultAsync(l =>
                    l.ClienteId == 3573 &&
                    l.CidadeId == cidadeId &&
                    l.EstadoId == estadoId);

            if (leadEsporadico != null)
            {
                Console.WriteLine(
                    $"ℹ️ Cliente {clienteId} sem lead próprio — usando LeadTime do Esporádico ({leadEsporadico.DiasLead} dias)."
                );
                return leadEsporadico.DiasLead;
            }

            // 3️⃣ Nenhum lead time configurado
            Console.WriteLine(
                $"⚠️ Nenhum LeadTime encontrado para Cliente {clienteId}, Cidade {cidadeId}, Estado {estadoId}, nem para Esporádico."
            );

            return 0;
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

    }
}
