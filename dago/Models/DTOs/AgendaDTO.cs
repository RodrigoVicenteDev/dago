namespace dago.Models.DTOs
{
    public class AgendaDTO
    {
        public int CtrcId { get; set; }
        public int TipoAgendaId { get; set; } // FK para TiposAgenda
        public DateTime DataAgenda { get; set; }
    }

}
