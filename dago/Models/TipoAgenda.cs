using System.ComponentModel.DataAnnotations;

namespace dago.Models
{
    public class TipoAgenda
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        // Relacionamento 1:N → Agendas
        public ICollection<Agenda> Agendas { get; set; } = new List<Agenda>();
    }
}
