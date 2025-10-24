using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using dago.Models;
using dago.Models.DTOs;
using dago.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace dago.Services
{
    public class AuthService
    {
        private readonly UsuarioRepository _repo;
        private readonly HashService _hash;
        private readonly IConfiguration _config;

        public AuthService(UsuarioRepository repo, HashService hash, IConfiguration config)
        {
            _repo = repo;
            _hash = hash;
            _config = config;
        }

        public async Task<LoginResponstaDTO> LoginAsync(LoginDTO dto)
        {
            var usuario = await _repo.ObterPorEmailComRelacionamentosAsync(dto.Email);
            if (usuario == null)
                throw new Exception("Usuário não encontrado.");

            if (!_hash.Verificar(dto.Senha, usuario.Senha))
                throw new Exception("Senha incorreta.");

            var token = GerarToken(usuario);

            // monta o DTO de resposta
            var resp = new LoginResponstaDTO
            {
                Token = token,
                Usuario = new LoginResponstaDTO.UsuarioResumo
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Cargo = usuario.Cargo?.Nome,
                    Clientes = usuario.Clientes.Select(c => new LoginResponstaDTO.ClienteResumo
                    {
                        Id = c.Id,
                        Nome = c.Nome,
                        Cnpj = c.Cnpj
                    }).ToList()
                }
            };

            return resp;
        }

        private string GerarToken(Usuario usuario)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                // facilita ler o Id depois via ClaimTypes.NameIdentifier
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim("nome", usuario.Nome),
                new Claim("cargoId", usuario.CargoId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(Convert.ToDouble(_config["Jwt:ExpireHours"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
