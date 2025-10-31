using System.ComponentModel.DataAnnotations;

namespace dago.Models
{
    public class TipoOcorrencia
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

       
        // Relacionamentos 1:N
       
        public ICollection<OcorrenciaAtendimento> OcorrenciasAtendimento { get; set; } = new List<OcorrenciaAtendimento>();
    }
}
