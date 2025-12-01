namespace dago.Models.DTOs.Views
{
    public class CtrcsParadosGruDTO
    {
        public int Id { get; set; }
        public int CtrcId { get; set; }
        public required string Numero { get; set; }
        public DateTime Data { get; set; }
        public int ClienteId { get; set; }
        public required string NumeroNotaFiscal { get; set; } 
        public required string Nome { get; set; }
        public int UnidadeId { get; set; }
        public required string Descricao { get; set; }
        public int Quantidade { get; set; }
    }
}
