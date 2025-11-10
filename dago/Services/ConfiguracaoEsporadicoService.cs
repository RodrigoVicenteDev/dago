using dago.Data;
using dago.Models;
using dago.Models.DTOs;
using dago.Repository;
using Microsoft.EntityFrameworkCore;

namespace dago.Services
{
    public class ConfiguracaoEsporadicoService : IConfiguracaoEsporadicoService
    {
        private readonly IConfiguracaoEsporadicoRepository _repo;
        private readonly AppDbContext _db;

        public ConfiguracaoEsporadicoService(IConfiguracaoEsporadicoRepository repo, AppDbContext db)
        {
            _repo = repo;
            _db = db;
        }

        public async Task<ConfiguracaoEsporadicoDTO?> GetAsync()
        {
            var config = await _repo.GetAsync();
            if (config == null) return null;

            return new ConfiguracaoEsporadicoDTO
            {
                ClientesExcluidos = config.ClientesExcluidos.Select(c => c.ClienteId).ToList(),
                UnidadesExcluidas = config.UnidadesExcluidas.Select(u => u.UnidadeId).ToList(),
                DestinatariosExcluidos = config.DestinatariosExcluidos.Select(d => d.Destinatario).ToList()
            };
        }

        public async Task<ConfiguracaoEsporadicoDTO> SaveAsync(ConfiguracaoEsporadicoDTO dto)
        {
            // Limpa configuração anterior
            await _repo.ClearAsync();

            var config = new ConfiguracaoEsporadico
            {
                ClientesExcluidos = dto.ClientesExcluidos
                    .Select(id => new ConfiguracaoEsporadicoCliente { ClienteId = id }).ToList(),

                UnidadesExcluidas = dto.UnidadesExcluidas
                    .Select(id => new ConfiguracaoEsporadicoUnidade { UnidadeId = id }).ToList(),

                DestinatariosExcluidos = dto.DestinatariosExcluidos
                    .Select(nome => new ConfiguracaoEsporadicoDestinatario { Destinatario = nome }).ToList()
            };

            _db.ConfiguracoesEsporadico.Add(config);
            await _db.SaveChangesAsync();

            return dto;
        }
    }
}
