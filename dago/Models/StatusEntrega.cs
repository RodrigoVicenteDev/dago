using System.ComponentModel.DataAnnotations;

namespace dago.Models
{
    public class StatusEntrega
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Descricao { get; set; }

        // Relacionamento 1:N → CTRCs
        public ICollection<Ctrc> Ctrcs { get; set; } = new List<Ctrc>();
    }
}

