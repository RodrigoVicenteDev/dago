using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dago.Models
{
    public class Cidade
    {
        [Key]
        public int Id { get; set; }


        [Required, MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [ForeignKey(nameof(Estado))]
        public int EstadoId { get; set; }

        public Estado Estado { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(TipoRegiao))]
        public int TipoRegiaoId { get; set; }

        public TipoRegiao TipoRegiao { get; set; } = null!;
        public ICollection<LeadTimeCliente> LeadTimesCliente { get; set; } = new List<LeadTimeCliente>();

        public ICollection<Ctrc> Ctrcs { get; set; } = new List<Ctrc>();
    }
}

