namespace dago.Models.DTOs
{
    public class StatusEntregaDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }

    public class TipoOcorrenciaDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }

    public class CtrcGridLookupsDTO
    {
        public List<StatusEntregaDTO> Statuses { get; set; } = new();
        public List<TipoOcorrenciaDTO> TiposOcorrencia { get; set; } = new();
    }
}
