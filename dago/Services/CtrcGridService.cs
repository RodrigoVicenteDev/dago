using dago.Data;
using dago.Models;
using dago.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace dago.Services
{
    public class CtrcGridService : ICtrcGridService
    {
        private readonly AppDbContext _db;

        public CtrcGridService(AppDbContext db)
        {
            _db = db;
        }

        // =============================
        // LISTAGEM PRINCIPAL PARA O GRID
        // =============================
        public async Task<List<CtrcGridDTO>> ListarAsync(int usuarioId, string cargo, DateTime? dataInicio, DateTime? dataFim)
        {
            // 🔹 Sempre trabalhar com datas puras (sem hora)
            var fim = dataFim?.Date ?? DateTime.Today;
            var inicio = dataInicio?.Date ?? fim.AddDays(-30);

            if ((fim - inicio).TotalDays > 60)
                throw new InvalidOperationException("O período máximo permitido é de 60 dias.");

            var query = _db.Ctrcs
                .Include(c => c.Cliente)
                .Include(c => c.CidadeDestino).ThenInclude(ci => ci.Estado)
                .Include(c => c.Unidade)
                .Include(c => c.StatusEntrega)
                .Include(c => c.OcorrenciasSistema)
                .Include(c => c.OcorrenciasAtendimento)
                .Where(c => c.DataEmissao >= inicio && c.DataEmissao <= fim)
                .AsNoTracking();

            // 🔹 Filtro por cargo
            if (!string.Equals(cargo, "gerente", StringComparison.OrdinalIgnoreCase))
            {
                var clientesIds = await _db.Clientes
                    .Where(x => x.UsuarioId == usuarioId)
                    .Select(x => x.Id)
                    .ToListAsync();

                if (clientesIds.Any())
                    query = query.Where(c => clientesIds.Contains(c.ClienteId));
                else
                    return new List<CtrcGridDTO>();
            }

            // 🔹 Projeção otimizada
            var lista = await query
                .Select(c => new CtrcGridDTO
                {
                    Id = c.Id,
                    Ctrc = c.Numero,
                    DataEmissao = c.DataEmissao.Date,
                    Cliente = c.Cliente.Nome,
                    CidadeEntrega = c.CidadeDestino.Nome,
                    Uf = c.EstadoDestino.Sigla,
                    Unidade = c.Unidade.Nome,
                    Destinatario = c.Destinatario,
                    NumeroNotaFiscal = c.NumeroNotaFiscal ?? string.Empty,
                    UltimaOcorrenciaSistema = c.OcorrenciasSistema
                        .OrderByDescending(o => o.Data)
                        .Select(o => o.Descricao)
                        .FirstOrDefault(),
                    DataPrevistaEntrega = c.DataPrevistaEntrega.HasValue ? c.DataPrevistaEntrega.Value.Date : null,
                    DataEntregaRealizada = c.DataEntregaRealizada.HasValue ? c.DataEntregaRealizada.Value.Date : null,
                    Peso = c.Peso,
                    StatusEntregaId = c.StatusEntregaId,
                    StatusEntregaNome = c.StatusEntrega.Nome,
                    DesvioPrazoDias = c.DesvioPrazoDias,
                    NotasFiscais = c.NotasFiscais,
                    Observacao = c.Observacao
                })
                .ToListAsync();

            return lista;
        }

        // =====================================
        // LOOKUPS PARA COMBOS (Status + Ocorrência)
        // =====================================
        public async Task<CtrcGridLookupsDTO> ObterLookupsAsync()
        {
            var statuses = await _db.StatusesEntrega
                .AsNoTracking()
                .OrderBy(s => s.Id)
                .Select(s => new StatusEntregaDTO
                {
                    Id = s.Id,
                    Nome = s.Nome
                })
                .ToListAsync();

            var tiposOcorrencia = await _db.TiposOcorrencia
                .AsNoTracking()
                .OrderBy(t => t.Nome)
                .Select(t => new TipoOcorrenciaDTO
                {
                    Id = t.Id,
                    Nome = t.Nome
                })
                .ToListAsync();

            return new CtrcGridLookupsDTO
            {
                Statuses = statuses,
                TiposOcorrencia = tiposOcorrencia
            };
        }

        // ============================
        // UPDATE INLINE (edição do grid)
        // ============================
        public async Task AtualizarAsync(int ctrcId, CtrcGridUpdateDTO dto)
        {
            var ctrc = await _db.Ctrcs.FirstOrDefaultAsync(c => c.Id == ctrcId);
            if (ctrc == null)
                throw new InvalidOperationException("CTRC não encontrado.");

            bool recalcularPrazo = false;

            // 🔹 Atualiza DataEntregaRealizada
            if (dto.DataEntregaRealizada.HasValue)
            {
                ctrc.DataEntregaRealizada = dto.DataEntregaRealizada.Value.Date;
                recalcularPrazo = true;
            }

            // 🔹 Atualiza Status
            if (dto.StatusEntregaId.HasValue && dto.StatusEntregaId.Value > 0)
                ctrc.StatusEntregaId = dto.StatusEntregaId.Value;

            // 🔹 Atualiza Observação
            if (dto.Observacao != null)
                ctrc.Observacao = dto.Observacao;

            // 🔹 Adiciona Ocorrência (texto livre)
            if (!string.IsNullOrWhiteSpace(dto.DescricaoOcorrenciaAtendimento))
            {
                var ocorr = new OcorrenciaAtendimento
                {
                    CtrcId = ctrc.Id,
                    Data = DateTime.Today,
                    Descricao = dto.DescricaoOcorrenciaAtendimento,
                    ReplicaClientes = false
                };
                await _db.OcorrenciasAtendimento.AddAsync(ocorr);
            }

            // 🔹 Recalcula desvio e status automático
            if (recalcularPrazo && ctrc.DataEntregaRealizada.HasValue)
            {
                var entrega = ctrc.DataEntregaRealizada.Value.Date;
                var prazo = ctrc.DataPrevistaEntrega?.Date ?? ctrc.DataEmissao.Date.AddDays(ctrc.LeadTimeDias);

                ctrc.DesvioPrazoDias = (entrega - prazo).Days;
                if (!dto.StatusEntregaId.HasValue)
                    ctrc.StatusEntregaId = ctrc.DesvioPrazoDias > 0 ? 3 : 1;
            }

            await _db.SaveChangesAsync();
        }
    }
}
