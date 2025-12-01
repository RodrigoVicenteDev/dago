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
        public async Task<List<CtrcGridDTO>> GetGridByCtrcs(List<string> ctrcs)
        {
            // 🔹 Sanitiza entrada
            ctrcs = ctrcs
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .ToList();

            if (ctrcs.Count == 0)
                return new List<CtrcGridDTO>();

            // 🔹 Query base (igual à do grid principal, exceto período)
            var lista = await _db.Ctrcs
                .Include(c => c.Cliente)
                .Include(c => c.CidadeDestino).ThenInclude(ci => ci.Estado)
                .Include(c => c.Unidade)
                .Include(c => c.StatusEntrega)
                .Include(c => c.OcorrenciasSistema)
                .Include(c => c.OcorrenciasAtendimento)
                .Include(c => c.Agendas)
                .Where(c => ctrcs.Contains(c.Numero))
                .AsNoTracking()
                .Select(c => new CtrcGridDTO
                {
                    Id = c.Id,
                    Ctrc = c.Numero,
                    DataEmissao = c.DataEmissao,

                    Cliente = c.Cliente.Nome,
                    Destinatario = c.Destinatario,
                    CidadeEntrega = c.CidadeDestino.Nome,
                    Uf = c.CidadeDestino.Estado.Sigla,
                    Unidade = c.Unidade.Nome,
                    NumeroNotaFiscal = c.NumeroNotaFiscal,

                    UltimaOcorrenciaSistema = c.OcorrenciasSistema
                        .OrderByDescending(o => o.Data)
                        .Select(o => o.Descricao)
                        .FirstOrDefault(),

                    UltimaDescricaoOcorrenciaAtendimento = c.OcorrenciasAtendimento
                        .OrderByDescending(o => o.Data)
                        .Select(o => o.Descricao)
                        .FirstOrDefault(),

                    DataPrevistaEntrega = c.DataPrevistaEntrega,
                    DataEntregaRealizada = c.DataEntregaRealizada,

                    Peso = c.Peso,
                    StatusEntregaId = c.StatusEntregaId,
                    StatusEntregaNome = c.StatusEntrega.Nome,

                    NotasFiscais = c.NotasFiscais,
                    Observacao = c.Observacao,

                    DataAgenda = c.Agendas
                        .OrderByDescending(a => a.Data)
                        .Select(a => (DateTime?)a.Data)
                        .FirstOrDefault(),

                    DesvioPrazoDias = c.DesvioPrazoDias, // será recalculado abaixo
                })
                .ToListAsync();

            // 🔹 Pós-processamento — Agora .Date pode ser usado
            foreach (var dto in lista)
            {
                dto.DataEmissao = dto.DataEmissao.Date;

                if (dto.DataPrevistaEntrega.HasValue)
                    dto.DataPrevistaEntrega = dto.DataPrevistaEntrega.Value.Date;

                if (dto.DataEntregaRealizada.HasValue)
                    dto.DataEntregaRealizada = dto.DataEntregaRealizada.Value.Date;

                if (dto.DataAgenda.HasValue)
                    dto.DataAgenda = dto.DataAgenda.Value.Date;

                // Recalcula desvio oficial
                if (!dto.DataEntregaRealizada.HasValue)
                {
                    dto.DesvioPrazoDias = null;
                    continue;
                }

                var entrega = dto.DataEntregaRealizada.Value.Date;

                var basePrazo =
                dto.DataAgenda
                ?? dto.DataPrevistaEntrega
                ?? dto.DataEmissao;

                dto.DesvioPrazoDias = (entrega - basePrazo).Days;

            }

            return lista;
        }


        // =============================
        // LISTAGEM PRINCIPAL PARA O GRID
        // =============================
        public async Task<List<CtrcGridDTO>> ListarAsync(int usuarioId, string cargo, DateTime? dataInicio, DateTime? dataFim)
        {
            var fim = dataFim?.Date ?? DateTime.Today;
            var inicio = dataInicio?.Date ?? fim.AddDays(-60);

            if ((fim - inicio).TotalDays > 60)
                throw new InvalidOperationException("O período máximo permitido é de 60 dias.");

            var query = _db.Ctrcs
                .Include(c => c.Cliente)
                .Include(c => c.CidadeDestino).ThenInclude(ci => ci.Estado)
                .Include(c => c.Unidade)
                .Include(c => c.StatusEntrega)
                .Include(c => c.OcorrenciasSistema)
                .Include(c => c.OcorrenciasAtendimento)
                .Include(c => c.Agendas)
                .Where(c => c.DataEmissao >= inicio && c.DataEmissao <= fim)
                .AsNoTracking();

            // 🔹 Filtro de acesso: apenas não-gerentes têm restrições
            if (!string.Equals(cargo, "gerente", StringComparison.OrdinalIgnoreCase))
            {
                var clientesIds = await _db.Clientes
                    .Where(x => x.UsuarioId == usuarioId)
                    .Select(x => x.Id)
                    .ToListAsync();

                if (!clientesIds.Any())
                    return new List<CtrcGridDTO>();

                bool isEsporadico = clientesIds.Contains(3573);

                Console.WriteLine(isEsporadico
                    ? "👀 Modo ESPORÁDICO: exibindo todos os CTRCs exceto os excluídos."
                    : "👀 Modo NORMAL: exibindo apenas clientes vinculados ao usuário.");

                // 🔸 Se NÃO for esporádico → filtra apenas clientes atribuídos
                if (!isEsporadico)
                    query = query.Where(c => clientesIds.Contains(c.ClienteId));

                // 🔹 Executa query base com projeção anônima (para incluir IDs e DTO juntos)
                var listaRaw = await query
                    .Select(c => new
                    {
                        ClienteId = c.ClienteId,
                        UnidadeId = c.UnidadeId,
                        Destinatario = c.Destinatario,
                        Dto = new CtrcGridDTO
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
                            UltimaDescricaoOcorrenciaAtendimento = c.OcorrenciasAtendimento
                                .OrderByDescending(o => o.Data)
                                .Select(o => o.Descricao)
                                .FirstOrDefault(),
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
                            Observacao = c.Observacao,
                            DataAgenda = c.Agendas
                                .OrderByDescending(a => a.Data)
                                .Select(a => (DateTime?)a.Data)
                                .FirstOrDefault(),
                        }
                    })
                    .ToListAsync();

                // 🔹 Recalcula o desvio aqui, seguindo a sua regra (M-K, mas K=Agenda se existir)
                foreach (var x in listaRaw)
                {
                    var dto = x.Dto;

                    if (dto.DataEntregaRealizada.HasValue)
                    {
                        var basePrazo =
                            (dto.DataAgenda.HasValue ? dto.DataAgenda.Value.Date
                            : dto.DataPrevistaEntrega.HasValue ? dto.DataPrevistaEntrega.Value.Date
                            : dto.DataEmissao.Date); // fallback extremo

                        dto.DesvioPrazoDias = (dto.DataEntregaRealizada.Value.Date - basePrazo).Days;
                    }
                    else
                    {
                        dto.DesvioPrazoDias = null;
                    }
                }

                // 🔹 Aplica filtros de configuração de esporádicos (somente se o usuário for esporádico)
                if (isEsporadico)
                {
                    var config = await _db.ConfiguracoesEsporadico
                        .Include(x => x.ClientesExcluidos)
                        .Include(x => x.UnidadesExcluidas)
                        .Include(x => x.DestinatariosExcluidos)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

                    if (config != null)
                    {
                        var clientesExcluidos = config.ClientesExcluidos.Select(c => c.ClienteId).ToHashSet();
                        var unidadesExcluidas = config.UnidadesExcluidas.Select(u => u.UnidadeId).ToHashSet();
                        var destinatariosExcluidos = config.DestinatariosExcluidos
                            .Select(d => d.Destinatario.ToUpperInvariant())
                            .ToHashSet();

                        Console.WriteLine("⚙️ Aplicando filtros de esporádico (em memória)");
                        Console.WriteLine($" - Clientes excluídos: {string.Join(", ", clientesExcluidos)}");
                        Console.WriteLine($" - Unidades excluídas: {string.Join(", ", unidadesExcluidas)}");
                        Console.WriteLine($" - Destinatários excluídos: {string.Join(", ", destinatariosExcluidos)}");

                        var totalAntes = listaRaw.Count;

                        listaRaw = listaRaw
                            .Where(x =>
                                !clientesExcluidos.Contains(x.ClienteId) &&
                                !unidadesExcluidas.Contains(x.UnidadeId) &&
                                !destinatariosExcluidos.Contains((x.Destinatario ?? string.Empty).ToUpperInvariant()))
                            .ToList();

                        Console.WriteLine($"📊 Filtro esporádico → antes: {totalAntes}, depois: {listaRaw.Count}");
                    }
                }

                // 🔹 Retorna apenas o DTO final
                return listaRaw.Select(x => x.Dto).ToList();
            }

            // 🔹 Caso o usuário seja gerente → retorna todos os CTRCs sem restrições
            var listaCompleta = await query
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
                  UltimaDescricaoOcorrenciaAtendimento = c.OcorrenciasAtendimento
                        .OrderByDescending(o => o.Data)
                        .Select(o => o.Descricao)
                        .FirstOrDefault(),

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
                  Observacao = c.Observacao,

                  // 🟢 NOVO CAMPO
                  DataAgenda = c.Agendas
                    .OrderByDescending(a => a.Data)
                    .Select(a => (DateTime?)a.Data)
                    .FirstOrDefault(),
              })
              .ToListAsync();

            foreach (var dto in listaCompleta)
            {
                if (dto.DataEntregaRealizada.HasValue)
                {
                    var basePrazo =
                        (dto.DataAgenda.HasValue ? dto.DataAgenda.Value.Date
                        : dto.DataPrevistaEntrega.HasValue ? dto.DataPrevistaEntrega.Value.Date
                        : dto.DataEmissao.Date);

                    dto.DesvioPrazoDias = (dto.DataEntregaRealizada.Value.Date - basePrazo).Days;
                }
                else
                {
                    dto.DesvioPrazoDias = null;
                }
            }

            return listaCompleta;
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
                    ReplicaClientes = false,
                    TipoOcorrenciaId = dto.TipoOcorrenciaId

                };
                await _db.OcorrenciasAtendimento.AddAsync(ocorr);
            }

            // 🔹 Recalcula desvio e status automático
            if (recalcularPrazo && ctrc.DataEntregaRealizada.HasValue)
            {
                var entrega = ctrc.DataEntregaRealizada.Value.Date;

                // 🔧 usa a agenda mais recente, se existir; senão, a data prevista (ou emissao+lead)
                var dataAgenda = await _db.Agendas
                    .Where(a => a.CtrcId == ctrc.Id)
                    .OrderByDescending(a => a.Data)
                    .Select(a => (DateTime?)a.Data)
                    .FirstOrDefaultAsync();

                var prazo = (dataAgenda?.Date)
                    ?? (ctrc.DataPrevistaEntrega?.Date)
                    ?? ctrc.DataEmissao.Date.AddDays(ctrc.LeadTimeDias);

                ctrc.DesvioPrazoDias = (entrega - prazo).Days;

                if (!dto.StatusEntregaId.HasValue)
                    ctrc.StatusEntregaId = ctrc.DesvioPrazoDias > 0 ? 3 : 1;
            }


            await _db.SaveChangesAsync();
        }
    }
}
