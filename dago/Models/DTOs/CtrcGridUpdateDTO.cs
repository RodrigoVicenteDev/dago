public class CtrcGridUpdateDTO
{
    public DateTime? DataEntregaRealizada { get; set; }
    public int? StatusEntregaId { get; set; }
    public string? Observacao { get; set; }

    // Texto livre da ocorrência de atendimento
    public string? DescricaoOcorrenciaAtendimento { get; set; }
}
