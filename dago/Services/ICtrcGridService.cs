using dago.Models.DTOs;

namespace dago.Services
{
    /// <summary>
    /// Serviço responsável pelas operações do grid de CTRCs (listagem, atualização e lookups).
    /// Todos os métodos consideram apenas datas puras (sem fuso horário).
    /// </summary>
    public interface ICtrcGridService
    {
        /// <summary>
        /// Lista os CTRCs visíveis ao usuário, aplicando filtros de cargo e período.
        /// O período máximo permitido é de 60 dias.
        /// </summary>
        /// <param name="usuarioId">ID do usuário autenticado.</param>
        /// <param name="cargo">Nome do cargo (Gerente, Atendente, etc.).</param>
        /// <param name="dataInicio">Data inicial do filtro.</param>
        /// <param name="dataFim">Data final do filtro.</param>
        /// <returns>Lista de CTRCs resumidos para exibição no grid.</returns>
        Task<List<CtrcGridDTO>> ListarAsync(
            int usuarioId,
            string cargo,
            DateTime? dataInicio,
            DateTime? dataFim);

        /// <summary>
        /// Retorna listas auxiliares para os combos do grid
        /// (Status de Entrega e Tipos de Ocorrência).
        /// </summary>
        Task<CtrcGridLookupsDTO> ObterLookupsAsync();

        /// <summary>
        /// Atualiza um CTRC diretamente do grid (edição inline).
        /// </summary>
        /// <param name="ctrcId">Identificador do CTRC.</param>
        /// <param name="dto">Objeto com os campos editáveis.</param>
        Task AtualizarAsync(int ctrcId, CtrcGridUpdateDTO dto);
    }
}
