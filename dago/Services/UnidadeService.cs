using dago.Models;
using dago.Repository;

namespace dago.Services
{
    public class UnidadeService
    {
        private readonly UnidadeRepository _repo;

        public UnidadeService(UnidadeRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Unidade>> ListarUnidadesAsync()
        {
            return await _repo.ListarAsync();
        }
    }
}
