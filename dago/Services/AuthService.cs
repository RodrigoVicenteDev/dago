using dago.Models;
using dago.Models.DTOs;
using dago.Repository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

        public async Task<string> LoginAsync(LoginDTO dto)
        {
            var usuario = await _repo.ObterPorEmailAsync(dto.Email);
            if (usuario == null)
                throw new Exception("Usuário não encontrado.");

            if (!_hash.Verificar(dto.Senha, usuario.Senha))
                throw new Exception("Senha incorreta.");

            // gerar token JWT
            return GerarToken(usuario);
        }

        private string GerarToken(Usuario usuario)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
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
