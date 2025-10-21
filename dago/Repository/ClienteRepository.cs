using dago.Data;
using dago.Models;
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

        public async Task<Cliente?> ObterPorIdjAsync(int id)
        {
            return await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<Cliente> CriarAsync(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return cliente;
        }

        public async Task<List<Cliente>> ObterTodosAsync()
        {
            return await _context.Clientes
                .Include(c => c.Usuario)
                .ToListAsync();
        }

        public async Task Deletar(Cliente cliente )
        {
            

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
        }
    }
}
