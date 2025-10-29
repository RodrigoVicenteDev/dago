using System.ComponentModel.DataAnnotations;

namespace dago.Models
{
    public class TipoOcorrencia
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Descricao { get; set; }

        // Relacionamentos 1:N
        public ICollection<OcorrenciaSistema> OcorrenciasSistema { get; set; } = new List<OcorrenciaSistema>();
        public ICollection<OcorrenciaAtendimento> OcorrenciasAtendimento { get; set; } = new List<OcorrenciaAtendimento>();
    }
}
