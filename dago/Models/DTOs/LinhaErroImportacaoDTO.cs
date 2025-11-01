namespace dago.Models.DTOs
{
    public class LinhaErroImportacaoDTO
    {
        public int Linha { get; set; }
        public string Ctrc { get; set; } = string.Empty;
        public string Erro { get; set; } = string.Empty;
    }
}
