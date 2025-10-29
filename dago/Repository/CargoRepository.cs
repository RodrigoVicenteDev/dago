using dago.Data;
using dago.Models;
using dago.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace dago.Repository
{
    public class CargoRepository
    {
        private readonly AppDbContext _context;

        public CargoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cargo?> ObterPorIDAsync(int id)
        {
            return await _context.Cargos.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<List<Cargo>> ListarAsync()
        {
            return await _context.Cargos.ToListAsync();
        }

        public async Task<Cargo?> ObterPorNomeAsync(string nome)
        {
            return await _context.Cargos.FirstOrDefaultAsync(c => c.Nome == nome);
        }

        public async Task<Cargo> CriarAsync(Cargo cargo)
        {
            _context.Cargos.Add(cargo);
            await _context.SaveChangesAsync();
            return cargo;
        }


        public async Task DeletarCargoAsync(Cargo cargo)
        {
            _context.Cargos.Remove(cargo);
            await _context.SaveChangesAsync();
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

        public async Task<Cargo?> ObterPorIdAsync(int id)
        {
            return await _context.Cargos
                .Include(c => c.Usuarios)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }

}
