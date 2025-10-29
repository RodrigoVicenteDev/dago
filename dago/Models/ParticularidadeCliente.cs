using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dago.Models
{
    public class ParticularidadeCliente
    {
        [Key]
        public int Id { get; set; }

        // FK 1:1 → CTRC
        [Required]
        [ForeignKey(nameof(Ctrc))]
        public int CtrcId { get; set; }
        public Ctrc Ctrc { get; set; } = null!;

        // Campos do diagrama
        [Required, MaxLength(50)]
        public string Remessa { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Loja { get; set; } = string.Empty;
    }
}
