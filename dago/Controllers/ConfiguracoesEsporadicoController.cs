using dago.Models.DTOs;
using dago.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dago.Controllers
{
    [ApiController]
    [Route("api/configuracoes-esporadico")]
    [Authorize]
    public class ConfiguracoesEsporadicoController : ControllerBase
    {
        private readonly IConfiguracaoEsporadicoService _service;

        public ConfiguracoesEsporadicoController(IConfiguracaoEsporadicoService service)
        {
            _service = service;
        }

        // GET api/configuracoes-esporadico
        [HttpGet]
        public async Task<ActionResult<ConfiguracaoEsporadicoDTO>> Get()
        {
            var result = await _service.GetAsync();
            if (result == null) return NotFound();
            return Ok(result);
        }

        // POST api/configuracoes-esporadico
        [HttpPost]
        public async Task<ActionResult<ConfiguracaoEsporadicoDTO>> Save([FromBody] ConfiguracaoEsporadicoDTO dto)
        {
            var result = await _service.SaveAsync(dto);
            return Ok(result);
        }
    }
}
