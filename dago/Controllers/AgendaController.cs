using dago.Models.DTOs;
using dago.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dago.Data;
namespace dago.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AgendaController : ControllerBase
    {
        private readonly AgendaService _agendaService;
        private readonly AppDbContext _context;

        public AgendaController(AgendaService agendaService, AppDbContext context)
        {
            _agendaService = agendaService;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarAgenda([FromBody] AgendaDTO dto)
        {
            try
            {
                await _agendaService.RegistrarAgendaAsync(dto);
                return Ok(new { message = "Agenda registrada com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("api/tiposagenda")]
        public async Task<IActionResult> GetTiposAgenda()
        {
            var tipos = await _context.TiposAgenda
                .AsNoTracking()
                .OrderBy(t => t.Id)
                .Select(t => new
                {
                    t.Id,
                    t.Nome
                })
                .ToListAsync();

            return Ok(tipos);
        }

        [HttpGet("ctrc/{ctrcId}")]
        public async Task<IActionResult> ListarPorCtrc(int ctrcId)
        {
            var agendas = await _agendaService.ListarPorCtrcAsync(ctrcId);
            return Ok(agendas);
        }

        [HttpGet("ctrc/{ctrcId}/ultima")]
        public async Task<IActionResult> ObterUltimaPorCtrc(int ctrcId)
        {
            var agenda = await _agendaService.ObterUltimaPorCtrcAsync(ctrcId);
            return Ok(agenda);
        }
    }
}
