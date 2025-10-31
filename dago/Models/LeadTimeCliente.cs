using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dago.Models
{
    public class LeadTimeCliente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Cliente))]
        public int ClienteId { get; set; }

        public Cliente Cliente { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(TipoRegiao))]
        public int TipoRegiaoId { get; set; }

        public TipoRegiao TipoRegiao { get; set; } = null!;
        [ForeignKey(nameof(RegiaoEstado))]
        public int RegiaoEstadoId { get; set; }
        public RegiaoEstado RegiaoEstado { get; set; } = null!;

        [Required]
        public int DiasLead { get; set; }
    }
}
