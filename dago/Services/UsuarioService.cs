using dago.Data;
using dago.Models;
using dago.Models.DTOs;
using dago.Repository;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace dago.Services
{
    public class UsuarioService
    {
        private readonly UsuarioRepository _repo;
        private readonly HashService _hash;
        private readonly AppDbContext _context;

        public UsuarioService(UsuarioRepository repo, HashService hash, AppDbContext context )
        {
            _repo = repo;
            _hash = hash;
            _context = context;
        }

        public async Task<List<Cliente>> BuscarClientes(List<int> ids)
        {

            var clientes = await _context.Clientes
                .Where(c => ids.Contains(c.Id))
                .ToListAsync();
            return clientes;
        }
        public async Task<Usuario?> CriarUsuarioAsync(Usuario usuario)
        {
            // Verifica se já existe e-mail
            var existente = await _repo.ObterPorEmailAsync(usuario.Email);
            if (existente != null)
                throw new Exception("Já existe um usuário com este e-mail.");

            // Criptografa a senha antes de salvar
            usuario.Senha = _hash.GerarHash(usuario.Senha);

          

            return await _repo.CriarAsync(usuario);
        }


        public async Task<Usuario> AtualizarUsuarioAsync(int id, AtualizarUsuarioDTO dto)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Clientes)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                throw new Exception("Usuário não localizado.");

            // 🔹 Atualiza campos básicos
            if (!string.IsNullOrWhiteSpace(dto.Nome))
                usuario.Nome = dto.Nome;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                usuario.Email = dto.Email;

            if (dto.CargoId.HasValue)
                usuario.CargoId = dto.CargoId.Value;

            if (!string.IsNullOrWhiteSpace(dto.NovaSenha))
                usuario.Senha = _hash.GerarHash(dto.NovaSenha);

            // 🔹 Atualiza clientes (somente se lista for enviada)
            if (dto.ClientesIds != null)
                await AtualizarClientesUsuario(usuario.Id, dto.ClientesIds);

            await _repo.AtualizarAsync(usuario);
            return usuario;
        }

        // 🔸 método privado para atualizar os clientes
        private async Task AtualizarClientesUsuario(int usuarioId, List<int> clientesIds)
        {
            // 1) Se null: não altera nada
            if (clientesIds is null) return;

            // 2) Remover todos (lista vazia)
            if (clientesIds.Count == 0)
            {
                var vinculados = await _context.Clientes
                    .Where(c => c.UsuarioId == usuarioId)
                    .ToListAsync();

                foreach (var c in vinculados)
                    c.UsuarioId = null;

                await _context.SaveChangesAsync();
                return;
            }

            // 3) Pegar IDs atualmente atribuídos ao usuário
            var atuaisIds = await _context.Clientes
                .Where(c => c.UsuarioId == usuarioId)
                .Select(c => c.Id)
                .ToListAsync();

            var paraAdicionar = clientesIds.Except(atuaisIds).ToList();
            var paraRemover = atuaisIds.Except(clientesIds).ToList();

            // 3a) Validar conflitos: não permitir clientes já atribuídos a outro usuário
            if (paraAdicionar.Count > 0)
            {
                var ocupados = await _context.Clientes
                    .Where(c => paraAdicionar.Contains(c.Id) &&
                                c.UsuarioId.HasValue &&
                                c.UsuarioId != usuarioId)
                    .Select(c => new { c.Id, c.Nome })
                    .ToListAsync();

                if (ocupados.Count > 0)
                {
                    var nomes = string.Join(", ", ocupados.Select(o => o.Nome));
                    throw new Exception($"Os seguintes clientes já estão atribuídos a outro usuário: {nomes}");
                }
            }

            // 3b) Atribuir novos
            if (paraAdicionar.Count > 0)
            {
                var novos = await _context.Clientes
                    .Where(c => paraAdicionar.Contains(c.Id))
                    .ToListAsync();

                foreach (var c in novos)
                    c.UsuarioId = usuarioId;
            }

            // 3c) Remover vínculos que saíram
            if (paraRemover.Count > 0)
            {
                var remover = await _context.Clientes
                    .Where(c => paraRemover.Contains(c.Id))
                    .ToListAsync();

                foreach (var c in remover)
                    c.UsuarioId = null;
            }

            await _context.SaveChangesAsync();
        }
            
        
        public async Task DeletarUsuarioAsync(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u=> u.Clientes)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                throw new Exception("Usuario não encontrado");

            if (usuario.Clientes != null && usuario.Clientes.Any())
            {
                foreach (var cliente in usuario.Clientes)
                    cliente.UsuarioId = null;
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();


        }
        public async Task<List<UsuarioDTO>> ListarUsuariosAsync()
        {
            var usuarios = await _repo.ObterTodosAsync();
            return usuarios.Select(u => new UsuarioDTO
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                Cargo = u.Cargo == null ? null : new CargoDTO
                {
                    Id = u.Cargo.Id,
                    Nome = u.Cargo.Nome
                },
                Clientes = u.Clientes.Select(c => new ClienteDTO
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Cnpj = c.Cnpj
                }).ToList()
            }).ToList();
        }


        

        public async Task<UsuarioDTO> ObterUsuarioPorIdAsync(int id)
        {
            var usuario = await _repo.ObterPorIdAsync(id);

            if (usuario == null)
                throw new Exception("Usuário não encontrado.");

            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Cargo = usuario.Cargo == null ? null : new CargoDTO
                {
                    Id = usuario.Cargo.Id,
                    Nome = usuario.Cargo.Nome
                },
                Clientes = usuario.Clientes.Select(c => new ClienteDTO
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Cnpj = c.Cnpj
                }).ToList()
            };
        }

        public async Task AlterarSenhaAsync(int id, AlterarSenhaDTO dto)
        {
            var usuario = await _repo.ObterPorIdAsync(id);
            if (usuario == null)
                throw new Exception("Usuário não encontrado.");

            // Verifica senha atual
            if (!_hash.Verificar(dto.SenhaAtual, usuario.Senha))
                throw new Exception("Senha atual incorreta.");

            // Gera hash da nova senha
            usuario.Senha = _hash.GerarHash(dto.NovaSenha);
            await _repo.AtualizarAsync(usuario);
        }
    }
}
