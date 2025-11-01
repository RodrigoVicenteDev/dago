using dago.Data;
using dago.Models;
using Microsoft.EntityFrameworkCore;

namespace dago.Repository
{
    public interface ICtrcRepository
    {
        Task<bool> CtrcExisteAsync(string numero);
        Task AddCtrcAsync(Ctrc ctrc);
        Task AddOcorrenciaSistemaAsync(OcorrenciaSistema ocorrencia);
        Task<int> SaveChangesAsync();
    }

    public class CtrcRepository : ICtrcRepository
    {
        private readonly AppDbContext _db;
        public CtrcRepository(AppDbContext db) => _db = db;

        public async Task<bool> CtrcExisteAsync(string numero) =>
            await _db.Ctrcs.AsNoTracking().AnyAsync(c => c.Numero == numero);

        public async Task AddCtrcAsync(Ctrc ctrc) => await _db.Ctrcs.AddAsync(ctrc);

        public async Task AddOcorrenciaSistemaAsync(OcorrenciaSistema ocorrencia) =>
            await _db.OcorrenciasSistema.AddAsync(ocorrencia);

        public async Task<int> SaveChangesAsync() => await _db.SaveChangesAsync();
    }
}
