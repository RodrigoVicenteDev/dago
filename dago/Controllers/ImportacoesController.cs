using dago.Models.DTOs;
using dago.Services;
using Microsoft.AspNetCore.Mvc;

namespace dago.Controllers
{
    [ApiController]
    [Route("api/importacoes")]
    public class ImportacoesController : ControllerBase
    {
        private readonly ICtrcImportService _service;

        public ImportacoesController(ICtrcImportService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lê e valida um CSV de CTRCs, retornando uma prévia dos dados limpos.
        /// </summary>
        /// <param name="request">Arquivo CSV e código da praça</param>
        [HttpPost("ctrc/preview")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Preview([FromForm] ImportarCtrcRequest request)
        {
            if (request.Arquivo == null || request.Arquivo.Length == 0)
                return BadRequest("Arquivo CSV é obrigatório.");

            var (token, preview) = await _service.PreviewAsync(request.Arquivo.OpenReadStream(), request.Praca);
            return Ok(new { token, preview });
        }

        /// <summary>
        /// Confirma e grava os dados previamente validados.
        /// </summary>
        /// <param name="token">Token da pré-visualização gerado na etapa anterior</param>
        [HttpPost("ctrc/confirmar/{token}")]
        public async Task<IActionResult> Confirmar(string token)
        {
            var (inseridos, erros) = await _service.ConfirmAsync(token);
            return Ok(new { inseridos, totalErros = erros.Count, erros });
        }
    }
}
