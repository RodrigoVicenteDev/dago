using dago.Data;
using dago.Models;
using Microsoft.EntityFrameworkCore;

namespace dago.Repository
{
    public class ConfiguracaoEsporadicoRepository : IConfiguracaoEsporadicoRepository
    {
        private readonly AppDbContext _db;

        public ConfiguracaoEsporadicoRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ConfiguracaoEsporadico?> GetAsync()
        {
            return await _db.ConfiguracoesEsporadico
                .Include(c => c.ClientesExcluidos)
                .Include(c => c.UnidadesExcluidas)
                .Include(c => c.DestinatariosExcluidos)
                .FirstOrDefaultAsync();
        }

        public async Task SaveAsync(ConfiguracaoEsporadico configuracao)
        {
            _db.ConfiguracoesEsporadico.Update(configuracao);
            await _db.SaveChangesAsync();
        }

        public async Task ClearAsync()
        {
            var existente = await _db.ConfiguracoesEsporadico
                .Include(c => c.ClientesExcluidos)
                .Include(c => c.UnidadesExcluidas)
                .Include(c => c.DestinatariosExcluidos)
                .FirstOrDefaultAsync();

            if (existente != null)
            {
                _db.ConfiguracoesEsporadico.Remove(existente);
                await _db.SaveChangesAsync();
            }
        }
    }
}
