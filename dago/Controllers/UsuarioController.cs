using dago.Models;
using dago.Models.DTOs;
using dago.Services;
using Microsoft.AspNetCore.Mvc;

namespace dago.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _service;

        public UsuarioController(UsuarioService service)
        {
            _service = service;
        }

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] CadastrarUsuarioDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var clientes = await _service.BuscarClientes(dto.ClientesID);

            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Senha = dto.Senha, // ainda em texto, será hasheada no service
                CargoId = dto.CargoId,
                Clientes = clientes,
            };

            try
            {
                var novoUsuario = await _service.CriarUsuarioAsync(usuario);

                return Ok(new
                {
                    mensagem = "Usuário cadastrado com sucesso!",
                    usuario = new
                    {
                        novoUsuario.Id,
                        novoUsuario.Nome,
                        novoUsuario.Email,
                        novoUsuario.CargoId
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }

            

        }

        [HttpPatch("atualizar/{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarUsuarioDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var atualizado = await _service.AtualizarUsuarioAsync(id, dto);
                return Ok(new
                {
                    atualizado.Id,
                    atualizado.Nome,
                    atualizado.Email,
                    atualizado.CargoId,
                }
                    );
            }
            catch (Exception ex)
            {

               return BadRequest(new {erro = ex.Message});
            }
        }

        [HttpDelete("deletar/{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            try
            {
                await _service.DeletarUsuarioAsync(id);
                return Ok(new { mensagem =  "Usuário excluído com sucesso." });
            }
            catch (Exception ex)
            {

                return BadRequest(new {erro = ex.Message});
            }
        }

        [HttpGet("listartodos")]
        public async Task<IActionResult> Listar()
        {
            var usuarios = await _service.ListarUsuariosAsync();
            return Ok(usuarios);
        }

        [HttpGet("busca/{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            try
            {
                var usuario = await _service.ObterUsuarioPorIdAsync(id);
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return NotFound(new { erro = ex.Message });
            }
        }

        [HttpPatch("alterarsenha/{id}")]
        public async Task<IActionResult> AlterarSenha(int id, [FromBody] AlterarSenhaDTO dto)
        {
            try
            {
                await _service.AlterarSenhaAsync(id, dto);
                return Ok(new { mensagem = "Senha alterada com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

    }
}
