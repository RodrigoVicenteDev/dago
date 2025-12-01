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
            // 🔹 Extrai o ID e cargo do JWT
            var usuarioIdClaim =
                User.FindFirst("sub") ??
                User.FindFirst(ClaimTypes.NameIdentifier) ??
                User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

            var cargoIdClaim = User.FindFirst("cargoId");

            var usuarioId = int.TryParse(usuarioIdClaim?.Value, out var id) ? id : 0;
            var cargoId = int.TryParse(cargoIdClaim?.Value, out var cid) ? cid : 0;

            var cargo = cargoId switch
            {
                1 => "Administrador",
                2 => "Gerente",
                3 => "Supervisor",
                4 => "Atendente",
                _ => "Atendente"
            };

            // 🔹 Padroniza datas para “somente data”
            var inicio = dataInicio?.Date;
            var fim = dataFim?.Date;

            Console.WriteLine("──────────────────────────────────────────────");
            Console.WriteLine($"🔐 JWT Claims → UsuarioId={usuarioId}, CargoId={cargoId} ({cargo})");
            Console.WriteLine($"📅 Filtro de datas: {inicio:yyyy-MM-dd} → {fim:yyyy-MM-dd}");
            Console.WriteLine("──────────────────────────────────────────────");

            var lista = await _gridService.ListarAsync(usuarioId, cargo, inicio, fim);

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

        [HttpPost("grid-por-lista")]
        public async Task<IActionResult> GridPorLista([FromBody] List<string> ctrcs)
        {
            if (ctrcs == null || ctrcs.Count == 0)
                return BadRequest("Lista de CTRCs vazia.");

            var lista = await _gridService.GetGridByCtrcs(ctrcs);

            return Ok(lista);
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

            // 🔹 Garante que a data venha sem hora
            if (dto.DataEntregaRealizada.HasValue)
                dto.DataEntregaRealizada = dto.DataEntregaRealizada.Value.Date;

            await _gridService.AtualizarAsync(id, dto);
            return NoContent();
        }
    }
}
