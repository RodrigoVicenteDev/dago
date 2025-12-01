using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/painel")]
public class PainelAvisosController : ControllerBase
{
    private readonly PainelAvisosService _service;

    public PainelAvisosController(PainelAvisosService service)
    {
        _service = service;
    }

    [HttpGet("avisos")]
    public async Task<IActionResult> GetPainel()
    {
        var result = await _service.GetPainelAsync();
        return Ok(result);
    }
}
