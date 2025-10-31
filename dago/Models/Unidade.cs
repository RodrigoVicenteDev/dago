using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dago.Models
{
    public class Unidade
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(120)]
        public string Nome { get; set; } = string.Empty;

        // FK → Estado (a unidade pertence a um estado)
        [Required]
        [ForeignKey(nameof(Estado))]
        public int EstadoId { get; set; }
        public Estado Estado { get; set; } = null!;

        // 1:N → CTRCs
        public ICollection<Ctrc> Ctrcs { get; set; } = new List<Ctrc>();
    }
}
