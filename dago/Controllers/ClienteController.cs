using dago.Models;
using dago.Models.DTOs;
using dago.Services;
using Microsoft.AspNetCore.Mvc;

namespace dago.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly ClienteService _service;

        public ClienteController(ClienteService service)
        {
            _service = service;
        }

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] CadastrarClienteDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var cliente = new Cliente
            {
                Nome = dto.Nome,
                Cnpj = dto.Cnpj,
                UsuarioId = dto.UsuarioId
            };

                 try
            {
                var novo = await _service.CriarClienteAsync(cliente);
                return Ok(new
                {
                    mensagem = "Cliente cadastrado com sucesso!",
                    cliente = new
                    {
                        novo.Id,
                        novo.Nome,
                        novo.Cnpj,
                        novo.UsuarioId
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
  
        }

        [HttpDelete("deletar/{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            try
            {
                await _service.DeletarClienteAsync(id);
                return Ok(new {message = "Cliente excluído com sucesso." });
            }
            catch (Exception ex)
            {

                return BadRequest(new {erro = ex.Message});
            }
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var clientes = await _service.ListarClientesAsync();
            return Ok(clientes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            try
            {
                var cliente = await _service.ObterClientePorIdAsync(id);
                return Ok(cliente);
            }
            catch (Exception ex)
            {
                return NotFound(new { erro = ex.Message });
            }
        }
    }
}
