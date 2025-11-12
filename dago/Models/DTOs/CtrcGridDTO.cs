namespace dago.Models.DTOs
{
    public class CtrcGridDTO
    {
        public int Id { get; set; }
        public string Ctrc { get; set; } = string.Empty;
        public DateTime DataEmissao { get; set; }

        public string Cliente { get; set; } = string.Empty;
        public string CidadeEntrega { get; set; } = string.Empty;
        public string Uf { get; set; } = string.Empty;
        public string Unidade { get; set; } = string.Empty;

        public string Destinatario { get; set; } = string.Empty;
        public string NumeroNotaFiscal { get; set; } = string.Empty;

        public string? UltimaOcorrenciaSistema { get; set; }
        public DateTime? DataPrevistaEntrega { get; set; }
        public DateTime? DataEntregaRealizada { get; set; }

        public decimal Peso { get; set; }

        public int StatusEntregaId { get; set; }

        public DateTime? DataAgenda { get; set; }
        public string StatusEntregaNome { get; set; } = string.Empty;
        public int? DesvioPrazoDias { get; set; }

        public string? NotasFiscais { get; set; }

        // Ocorrência de atendimento (última)
        public int? UltimaOcorrenciaAtendimentoId { get; set; }
        public int? UltimoTipoOcorrenciaAtendimentoId { get; set; }
        public string? UltimaDescricaoOcorrenciaAtendimento { get; set; }

        // Observação do CTRC
        public string? Observacao { get; set; }
    }
}
