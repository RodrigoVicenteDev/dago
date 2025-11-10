using dago.Models.DTOs;

namespace dago.Services
{
    public interface IConfiguracaoEsporadicoService
    {
        Task<ConfiguracaoEsporadicoDTO?> GetAsync();
        Task<ConfiguracaoEsporadicoDTO> SaveAsync(ConfiguracaoEsporadicoDTO dto);
    }
}
