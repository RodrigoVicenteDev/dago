using dago.Data;
using dago.Models;
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

        public async Task<List<Cargo>> ObterTodosAsync()
        {
            return await _context.Cargos
                .Include(c => c.Usuarios) // opcional: traz os usuários que têm esse cargo
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
