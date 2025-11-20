namespace dago.Models.DTOs
{
    public class CtrcCsvRow
    {
        public string SerieNumeroCtrc { get; set; } = string.Empty;
        public string TipoDocumento { get; set; } = string.Empty; // usado só para filtro
        public string DataEmissao { get; set; } = string.Empty;
        public string PracaExpedidora { get; set; } = string.Empty; // usado só para filtro
        public string ClienteRemetente { get; set; } = string.Empty;
        public string CnpjRemetente { get; set; } = string.Empty;
        public string ClienteDestinatario { get; set; } = string.Empty;
        public string CidadeEntrega { get; set; } = string.Empty;
        public string UfEntrega { get; set; } = string.Empty;
        public string UnidadeReceptora { get; set; } = string.Empty;
        public string NumeroNotaFiscal { get; set; } = string.Empty;
        public string DataUltimaOcorrencia { get; set; } = string.Empty;
        public string DescricaoUltimaOcorrencia { get; set; } = string.Empty;
        public string DataEntregaRealizada { get; set; } = string.Empty;
        public string NotasFiscais { get; set; } = string.Empty;
        public string PesoCalculadoKg { get; set; } = string.Empty;
    }
}
