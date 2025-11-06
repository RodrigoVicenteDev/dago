namespace dago.Models.DTOs
{
    public class LinhaErroImportacaoDTO
    {
        public int Linha { get; set; }
        public string? Ctrc { get; set; }
        public string Erro { get; set; } = string.Empty;

        // Novos campos usados no import service:
        public string? Severidade { get; set; }      // "Critico", "Alerta", etc.
        public bool CorrigidoAutomaticamente { get; set; } = false;
        public object? Payload { get; set; }         // guarda os dados crus da linha
    }
}
