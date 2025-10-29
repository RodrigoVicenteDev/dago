using dago.Models;
using dago.Models.DTOs;
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
            var cliente = await _repo.ObterPorIdAsync(id);

            if (cliente == null)
                throw new Exception("Cliente não encontrado.");

            await _repo.Deletar(cliente);
        }

        public async Task<List<ClienteDTO>> ListarClientesAsync()
        {
            return await _repo.ObterTodosAsync();
        }

        public async Task<ClienteDTO> ObterClientePorIdAsync(int id)
        {
            var cliente = await _repo.ObterPorIdAsyncDTO(id);
            if (cliente == null)
                throw new Exception("Cliente não encontrado.");
            return cliente;
        }

    }
}
