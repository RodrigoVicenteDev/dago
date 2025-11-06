using dago.Models.DTOs;
using dago.Services;
using Microsoft.AspNetCore.Mvc;

namespace dago.Controllers
{
    [ApiController]
    [Route("api/importacoes")]
    [Produces("application/json")]
    public class ImportacoesController : ControllerBase
    {
        private readonly ICtrcImportService _service;
        private readonly ILogger<ImportacoesController> _logger;

        public ImportacoesController(ICtrcImportService service, ILogger<ImportacoesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Lê e valida um arquivo CSV de CTRCs, retornando uma prévia dos dados limpos.
        /// </summary>
        /// <remarks>
        /// Envie o arquivo CSV no formato `multipart/form-data` com os campos:
        /// - arquivo: o CSV exportado do sistema SSW
        /// - praca: código da praça (ex: "GRU", "CRI")
        /// </remarks>
        /// <param name="request">Arquivo CSV e código da praça</param>
        /// <response code="200">Retorna token e prévia dos registros lidos</response>
        /// <response code="400">Se o arquivo estiver ausente ou inválido</response>
        /// <response code="500">Erro interno durante o processamento</response>
        [HttpPost("ctrc/preview")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Preview([FromForm] ImportarCtrcRequest request)
        {
            if (request.Arquivo == null || request.Arquivo.Length == 0)
                return BadRequest(new { erro = "Arquivo CSV é obrigatório." });

            try
            {
                using var stream = request.Arquivo.OpenReadStream();
                var (token, preview) = await _service.PreviewAsync(stream, request.Praca);

                return Ok(new
                {
                    sucesso = true,
                    token,
                    total = preview.Count,
                    preview
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante a leitura do CSV de CTRCs.");
                return StatusCode(500, new { sucesso = false, erro = ex.Message });
            }
        }

        /// <summary>
        /// Confirma e grava os dados previamente validados no banco de dados.
        /// </summary>
        /// <remarks>
        /// Após a pré-visualização, use o token retornado para confirmar a gravação dos CTRCs válidos.
        /// </remarks>
        /// <param name="token">Token da pré-visualização gerado na etapa anterior</param>
        /// <response code="200">Retorna quantos registros foram gravados e os erros pendentes</response>
        /// <response code="400">Se o token não for informado</response>
        /// <response code="500">Erro interno durante a gravação</response>
        [HttpPost("ctrc/confirmar/{token}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Confirmar(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest(new { erro = "Token é obrigatório." });

            try
            {
                var (inseridos, erros) = await _service.ConfirmAsync(token);

                return Ok(new
                {
                    sucesso = true,
                    inseridos,
                    totalErros = erros.Count,
                    erros
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Token de importação inválido ou expirado.");
                return BadRequest(new { sucesso = false, erro = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante a confirmação de importação de CTRCs.");
                return StatusCode(500, new { sucesso = false, erro = ex.Message });
            }
        }
    }

    /// <summary>
    /// Modelo usado no endpoint de importação de CTRCs (upload de CSV)
    /// </summary>
    public class ImportarCtrcRequest
    {
        /// <summary>Arquivo CSV exportado do sistema SSW</summary>
        public IFormFile? Arquivo { get; set; }

        /// <summary>Código da praça (ex: "GRU", "CRI")</summary>
        public string Praca { get; set; } = string.Empty;
    }
}
