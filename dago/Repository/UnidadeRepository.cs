using dago.Data;
using dago.Models;
using Microsoft.EntityFrameworkCore;

namespace dago.Repository
{
    public class UnidadeRepository
    {
        private readonly AppDbContext _context;

        public UnidadeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Unidade>> ListarAsync()
        {
            return await _context.Unidades
                .Include(u => u.Estado)
                .AsNoTracking()
                .OrderBy(u => u.Nome)
                .ToListAsync();
        }
    }
}
