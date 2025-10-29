using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dago.Models
{
    public class Agenda
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Ctrc))]
        public int CtrcId { get; set; }
        public Ctrc Ctrc { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(TipoAgenda))]
        public int TipoAgendaId { get; set; }
        public TipoAgenda TipoAgenda { get; set; } = null!;

        [Required]
        public DateTime Data { get; set; }


    }
}
