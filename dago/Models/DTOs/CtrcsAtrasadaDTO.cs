namespace dago.Models.DTOs.Views
{
    public class CtrcsAtrasadaDTO
    {
        public required string Numero { get; set; }
        public required string Destinatario { get; set; }
        public required string NumeroNotaFiscal { get; set; }
        public required string CidadeDestino { get; set; }
        public required string EstadoDestino { get; set; }
        public required string Cliente { get; set; }
        public int DiasAtraso { get; set; }

        public int UnidadeId { get; set; }

        public int ClienteId { get; set; }
        public string? UltimaOcorrenciaAtendimento { get; set; }
    }
}
