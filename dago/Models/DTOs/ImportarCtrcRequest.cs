namespace dago.Models.DTOs
{
    public class ImportarCtrcRequest
    {
        /// <summary>
        /// Arquivo CSV de CTRCs a ser importado.
        /// </summary>
        public IFormFile Arquivo { get; set; }

        /// <summary>
        /// Código da praça (ex: GRU, CTB, etc.) usado como filtro.
        /// </summary>
        public string Praca { get; set; } = string.Empty;
    }
}
