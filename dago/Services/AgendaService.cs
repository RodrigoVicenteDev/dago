using dago.Data;
using dago.Models;
using dago.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace dago.Services
{
    public class AgendaService
    {
        private readonly AppDbContext _context;

        public AgendaService(AppDbContext context)
        {
            _context = context;
        }

        // 🟢 1. Registrar nova agenda
        public async Task RegistrarAgendaAsync(AgendaDTO dto)
        {
            // Valida CTRC existente
            var ctrc = await _context.Ctrcs
                .Include(c => c.Agendas)
                .FirstOrDefaultAsync(c => c.Id == dto.CtrcId);

            if (ctrc == null)
                throw new Exception("CTRC não encontrado.");

            // Valida TipoAgenda existente
            var tipoAgenda = await _context.TiposAgenda.FindAsync(dto.TipoAgendaId);
            if (tipoAgenda == null)
                throw new Exception("Tipo de agenda inválido.");

            // Cria e adiciona nova agenda
            var agenda = new Agenda
            {
                CtrcId = dto.CtrcId,
                TipoAgendaId = dto.TipoAgendaId,
                Data = dto.DataAgenda
            };

            _context.Agendas.Add(agenda);
            await _context.SaveChangesAsync();
        }

        // 🟡 2. Listar todas as agendas de um CTRC (para histórico)
        public async Task<IEnumerable<Agenda>> ListarPorCtrcAsync(int ctrcId)
        {
            return await _context.Agendas
                .Include(a => a.TipoAgenda)
                .Where(a => a.CtrcId == ctrcId)
                .OrderByDescending(a => a.Data)
                .ToListAsync();
        }

        // 🔵 3. Obter a última agenda (para exibir no grid)
        public async Task<Agenda?> ObterUltimaPorCtrcAsync(int ctrcId)
        {
            return await _context.Agendas
                .Include(a => a.TipoAgenda)
                .Where(a => a.CtrcId == ctrcId)
                .OrderByDescending(a => a.Data)
                .FirstOrDefaultAsync();
        }
    }
}
