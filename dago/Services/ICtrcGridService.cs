using dago.Models.DTOs;

namespace dago.Services
{
    public interface ICtrcGridService
    {
        // 🔹 Agora com os 4 parâmetros
        Task<List<CtrcGridDTO>> ListarAsync(
            int usuarioId,
            string cargo,
            DateTime? dataInicio,
            DateTime? dataFim);

        Task<CtrcGridLookupsDTO> ObterLookupsAsync();
        Task AtualizarAsync(int ctrcId, CtrcGridUpdateDTO dto);
    }
}
