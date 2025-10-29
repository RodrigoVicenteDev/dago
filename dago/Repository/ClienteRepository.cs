using dago.Data;
using dago.Models;
using dago.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace dago.Repository
{
    public class ClienteRepository
    {
        private readonly AppDbContext _context;

        public ClienteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cliente?> ObterPorCnpjAsync(string cnpj)
        {
            return await _context.Clientes.FirstOrDefaultAsync(c => c.Cnpj == cnpj);
        }

        public async Task<Cliente?> ObterPorIdAsync(int id)
        {
            return await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<ClienteDTO?> ObterPorIdAsyncDTO(int id)
        {
            return await _context.Clientes
                .Include(c => c.Usuario)
                .Where(c => c.Id == id)
                .Select(c => new ClienteDTO
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Cnpj = c.Cnpj,
                    Usuario = c.Usuario == null ? null : new UsuarioResumoDTO
                    {
                        Id = c.Usuario.Id,
                        Nome = c.Usuario.Nome
                    }
                })
                .FirstOrDefaultAsync();
        }
        public async Task<Cliente> CriarAsync(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return cliente;
        }

        public async Task<List<ClienteDTO>> ObterTodosAsync()
        {
            return await _context.Clientes
                .Include(c => c.Usuario)
                .Select(c => new ClienteDTO
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Cnpj = c.Cnpj,
                    Usuario = c.Usuario == null ? null : new UsuarioResumoDTO
                    {
                        Id = c.Usuario.Id,
                        Nome = c.Usuario.Nome
                    }
                })
                .ToListAsync();
        }

        public async Task Deletar(Cliente cliente )
        {
            

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
        }
    }
}
