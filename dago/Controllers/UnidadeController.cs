using dago.Models;
using dago.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dago.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnidadeController : ControllerBase
    {
        private readonly UnidadeService _service;

        public UnidadeController(UnidadeService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retorna a lista de unidades cadastradas (com estado).
        /// </summary>
        [Authorize]
        [HttpGet("listar")]
        public async Task<IActionResult> Listar()
        {
            var unidades = await _service.ListarUnidadesAsync();
            return Ok(unidades.Select(u => new
            {
                u.Id,
                u.Nome,
                Estado = new
                {
                    u.Estado.Id,
                    u.Estado.Nome,
                    u.Estado.Sigla
                }
            }));
        }
    }
}
