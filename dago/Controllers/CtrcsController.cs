using dago.Models.DTOs;
using dago.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace dago.Controllers
{
    [ApiController]
    [Route("api/ctrcs")]
    public class CtrcsController : ControllerBase
    {
        private readonly ICtrcGridService _gridService;

        public CtrcsController(ICtrcGridService gridService)
        {
            _gridService = gridService;
        }

        /// <summary>
        /// Retorna a lista de CTRCs para o grid principal (AG Grid).
        /// Aplica filtros automáticos conforme cargo e período.
        /// </summary>
        [Authorize]
        [HttpGet("grid")]
        public async Task<IActionResult> GetGrid([FromQuery] DateTime? dataInicio, [FromQuery] DateTime? dataFim)
        {
            // 🔹 Extrai o ID do usuário autenticado
            var usuarioIdClaim =
                User.FindFirst("sub") ??
                User.FindFirst(ClaimTypes.NameIdentifier) ??
                User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

            var cargoIdClaim = User.FindFirst("cargoId");

            var usuarioId = int.TryParse(usuarioIdClaim?.Value, out var id) ? id : 0;
            var cargoId = int.TryParse(cargoIdClaim?.Value, out var cid) ? cid : 0;

            // 🔹 Mapeia cargoId → nome legível
            var cargo = cargoId switch
            {
                1 => "Administrador",
                2 => "Gerente",
                3 => "Supervisor",
                4 => "Atendente",
                _ => "Atendente"
            };

            Console.WriteLine("──────────────────────────────────────────────");
            Console.WriteLine($"🔐 JWT Claims → UsuarioId={usuarioId}, CargoId={cargoId} ({cargo})");
            Console.WriteLine($"📅 Filtro de datas: {dataInicio:yyyy-MM-dd} → {dataFim:yyyy-MM-dd}");
            Console.WriteLine("──────────────────────────────────────────────");

            // 🔹 Chama o serviço
            var lista = await _gridService.ListarAsync(usuarioId, cargo, dataInicio, dataFim);

            Console.WriteLine($"📤 Retornando {lista.Count} registros no grid\n");

            return Ok(lista);
        }

        /// <summary>
        /// Retorna listas auxiliares (StatusEntrega e TiposOcorrencia)
        /// para popular os combos do AG Grid.
        /// </summary>
        [Authorize]
        [HttpGet("lookups")]
        public async Task<IActionResult> GetLookups()
        {
            var lookups = await _gridService.ObterLookupsAsync();
            return Ok(lookups);
        }

        /// <summary>
        /// Atualiza campos editáveis do CTRC (data de entrega, status,
        /// observação e ocorrência de atendimento).
        /// </summary>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CtrcGridUpdateDTO dto)
        {
            if (dto == null)
                return BadRequest("Payload inválido.");

            await _gridService.AtualizarAsync(id, dto);
            return NoContent();
        }
    }
}
