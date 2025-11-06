using dago.Data;
using dago.Models;
using dago.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace dago.Services.Utils
{
    public class CtrcMapper
    {
        private readonly AppDbContext _db;
        private readonly CtrcNormalizer _normalizer;
        private readonly IBusinessDayService _businessDays;

        public CtrcMapper(AppDbContext db, CtrcNormalizer normalizer, IBusinessDayService businessDays)
        {
            _db = db;
            _normalizer = normalizer;
            _businessDays = businessDays;
        }

        public async Task<Ctrc> MapAsync(dynamic item)
        {
            string clienteNome = item.clienteRemetente;
            string destinatarioNome = item.clienteDestinatario;
            string cidadeEntrega = item.cidadeEntrega;
            string ufEntrega = item.ufEntrega;
            string unidadeReceptora = item.unidadeReceptora;

            var (cliente, cidade, estado, unidade) = await _normalizer.ResolverAsync(
                clienteNome, destinatarioNome, cidadeEntrega, ufEntrega, unidadeReceptora
            );

            DateTime? dataEmissao = ParseDate(item.dataEmissao);
            DateTime? dataEntrega = ParseDate(item.dataEntregaRealizada);
            DateTime? dataOcorrencia = ParseDate(item.dataUltimaOcorrencia);

            // === 1️⃣ LeadTimeDias ===
            int leadTimeDias = await CalcularLeadTimeDiasAsync(cliente.Id, cidade.Id, estado.Id);

            // === 2️⃣ DesvioPrazoDias ===
            int? desvioPrazoDias = null;
            if (dataEntrega.HasValue)
            {
                var dataReferencia = dataEmissao?.AddDays(leadTimeDias) ?? DateTime.Now;
                int diasUteis = _businessDays.BusinessDaysBetween(dataReferencia, dataEntrega.Value);
                desvioPrazoDias = diasUteis;
            }

            // === 3️⃣ StatusEntregaId ===
            int statusId = DefinirStatusEntrega(dataEntrega, dataEmissao, leadTimeDias, desvioPrazoDias);

            // === 4️⃣ Monta objeto Ctrc ===
            var ctrc = new Ctrc
            {
                Numero = item.ctrc ?? "",
                DataEmissao = dataEmissao ?? DateTime.Now,
                NumeroNotaFiscal = item.numeroNotaFiscal,
                ClienteId = cliente.Id,
                CidadeDestinoId = cidade.Id,
                EstadoDestinoId = estado.Id,
                UnidadeId = unidade.Id,
                Observacao = item.descricaoUltimaOcorrencia ?? "",
                Peso = (decimal)(item.pesoToneladas ?? 0),
                LeadTimeDias = leadTimeDias,
                DesvioPrazoDias = desvioPrazoDias,
                StatusEntregaId = statusId,
                Destinatario = destinatarioNome
            };

            return ctrc;
        }

        // === AUXILIARES ===

        private static DateTime? ParseDate(string? s)
        {
            if (DateTime.TryParseExact(s?.Trim(), "dd/MM/yyyy",
                new System.Globalization.CultureInfo("pt-BR"),
                System.Globalization.DateTimeStyles.None, out var d))
                return d;
            return null;
        }

        private async Task<int> CalcularLeadTimeDiasAsync(int clienteId, int cidadeId, int estadoId)
        {
            // Busca tipo e região da cidade
            var cidade = await _db.Cidades.AsNoTracking().FirstOrDefaultAsync(c => c.Id == cidadeId);
            if (cidade == null) return 0;

            var lead = await _db.LeadTimesCliente
                .FirstOrDefaultAsync(l =>
                    l.ClienteId == clienteId &&
                    l.TipoRegiaoId == cidade.TipoRegiaoId &&
                    l.RegiaoEstadoId == cidade.Estado.RegiaoEstadoId);

            if (lead == null)
            {
                // Usa o cliente "Esporádico" (ID = 3573)
                lead = await _db.LeadTimesCliente
                    .FirstOrDefaultAsync(l =>
                        l.ClienteId == 3573 &&
                        l.TipoRegiaoId == cidade.TipoRegiaoId &&
                        l.RegiaoEstadoId == cidade.Estado.RegiaoEstadoId);
            }

            return lead?.DiasLead ?? 0;
        }

        private int DefinirStatusEntrega(DateTime? dataEntrega, DateTime? dataEmissao, int leadTimeDias, int? desvioPrazoDias)
        {
            if (!dataEntrega.HasValue)
                return 2; // ainda não entregue (em atraso se passou prazo)

            if (desvioPrazoDias.HasValue && desvioPrazoDias > 0)
                return 3; // entregue com atraso

            return 1; // entregue no prazo
        }
    }
}
