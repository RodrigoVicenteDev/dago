namespace dago.Models.DTOs
{
    public class PlanilhaDTO
    {
        // Identificação principal
        public string Ctrc { get; set; } = string.Empty;
        public DateOnly Emissao { get; set; }

        // Dados do cliente
        public string Cliente { get; set; } = string.Empty;
        public string Destinatario { get; set; } = string.Empty;

        // Local de entrega
        public string CidadeEntrega { get; set; } = string.Empty;
        public string UF { get; set; } = string.Empty;
        public string Unidade { get; set; } = string.Empty; // Nome da unidade Dago

        // Dados fiscais
        public string NumeroNotaFiscal { get; set; } = string.Empty;
        public string NotasAgrupadas { get; set; } = string.Empty;

        // Ocorrências (sistema / atendimento)
        public string OcorrenciaSistema { get; set; } = string.Empty;
        public string OcorrenciaAtendimento { get; set; } = string.Empty;

        // Datas operacionais
        public DateOnly? LeadTime { get; set; }
        public DateOnly? Agenda { get; set; }
        public DateOnly? DataEntrega { get; set; }

        // Status e análise de prazo
        public string Status { get; set; } = string.Empty; // "ENTREGUE NO PRAZO", "COM ATRASO" etc.
        public int? DesvioPrazoDias { get; set; }          // positivo = atraso, negativo = adiantado
        public string Observacao { get; set; } = string.Empty; // campo livre de texto
    }

}
