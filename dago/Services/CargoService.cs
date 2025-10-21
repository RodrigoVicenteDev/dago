using dago.Models;
using dago.Repository;

namespace dago.Services
{
    public class CargoService
    {
        private readonly CargoRepository _repo;

        public CargoService(CargoRepository repo)
        {
            _repo = repo;
        }

        public async Task<Cargo> CriarCargoAsync(Cargo cargo)
        {
            var existente = await _repo.ObterPorNomeAsync(cargo.Nome);
            if (existente != null)
                throw new Exception("Já existe um cargo com este nome.");

            return await _repo.CriarAsync(cargo);
        }

        public async Task<List<Cargo>> ListarCargosAsync()
        {
            return await _repo.ListarAsync();
        }

       public async Task Deletar(int id)
        {
            var cargo = await _repo.ObterPorIDAsync(id);

            if (cargo == null)
                throw new Exception("Cargo não encontrado.");

            await _repo.DeletarCargoAsync(cargo);

        }


       

        public async Task<Cargo> ObterCargoPorIdAsync(int id)
        {
            var cargo = await _repo.ObterPorIdAsync(id);

            if (cargo == null)
                throw new Exception("Cargo não encontrado.");

            return cargo;
        }
    }
    
}
