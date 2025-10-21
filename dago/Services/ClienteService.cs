using dago.Models;
using dago.Repository;

namespace dago.Services
{
    public class ClienteService
    {
        private readonly ClienteRepository _repo;

        public ClienteService(ClienteRepository repo)
        {
            _repo = repo;
        }

        public async Task<Cliente> CriarClienteAsync(Cliente cliente)
        {
            // evita duplicação de CNPJ
            var existente = await _repo.ObterPorCnpjAsync(cliente.Cnpj);
            if (existente != null)
                throw new Exception("Já existe um cliente com este CNPJ.");

            return await _repo.CriarAsync(cliente);
        }

        public async Task DeletarClienteAsync(int id)
        {
            var cliente = await _repo.ObterPorIdjAsync(id);

            if (cliente == null)
                throw new Exception("Cliente não encontrado.");

            await _repo.Deletar(cliente);
        }

        public async Task<List<Cliente>> ListarClientesAsync()
        {
            return await _repo.ObterTodosAsync();
        }

        public async Task<Cliente> ObterClientePorIdAsync(int id)
        {
            var cliente = await _repo.ObterPorIdjAsync(id);
            if (cliente == null)
                throw new Exception("Cliente não encontrado.");
            return cliente;
        }

    }
}
