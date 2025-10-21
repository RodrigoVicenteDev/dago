using dago.Models;
using dago.Models.DTOs;
using dago.Services;
using Microsoft.AspNetCore.Mvc;

namespace dago.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CargoController : ControllerBase
    {
        private readonly CargoService _service;

        public CargoController(CargoService service)
        {
            _service = service;
        }

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] CadastrarCargoDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cargo = new Cargo
            {
                Nome = dto.Nome,

            };

            try
            {
                var novoCargo = await _service.CriarCargoAsync(cargo);
                return Ok(new
                {
                    mensagem = "Cargo cadastrado com sucesso!",
                    cargo = new { novoCargo.Id, novoCargo.Nome }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        [HttpGet("listar")]
        public async Task<IActionResult> Listar()
        {
            var cargos = await _service.ListarCargosAsync();
            return Ok(cargos);
        }

        [HttpDelete("deletar/{id}")]
        public async Task<IActionResult> Delete(int id)

        {
            try
            {
                await _service.Deletar(id);
                return Ok(new { mensagem = "Cliente excluído com sucesso." });

            }
            catch (Exception ex)
            {

                return BadRequest(new { erro = ex.Message });
            }
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            try
            {
                var cargo = await _service.ObterCargoPorIdAsync(id);
                return Ok(cargo);
            }
            catch (Exception ex)
            {
                return NotFound(new { erro = ex.Message });
            }

        }
    }
}
