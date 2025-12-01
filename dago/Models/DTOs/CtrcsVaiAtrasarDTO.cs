namespace dago.Models.DTOs.Views
{
    public class CtrcsVaiAtrasarDTO
    {
        public required string Numero { get; set; }
        public required string Destinatario { get; set; }
        public required string NumeroNotaFiscal { get; set; }
      
      
        public int UnidadeId { get; set; }

        public int ClienteId { get; set; }
        public  required string Cliente { get; set; }
      
    }
}
