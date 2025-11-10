using dago.Models;

namespace dago.Repository
{
    public interface IConfiguracaoEsporadicoRepository
    {
        Task<ConfiguracaoEsporadico?> GetAsync();
        Task SaveAsync(ConfiguracaoEsporadico configuracao);
        Task ClearAsync();
    }
}
