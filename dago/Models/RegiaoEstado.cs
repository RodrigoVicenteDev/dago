using System.ComponentModel.DataAnnotations;

namespace dago.Models
{
    public class RegiaoEstado
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Nome { get; set; } = string.Empty;

        [Required, MaxLength(10)]
        public string Sigla { get; set; } = string.Empty;

        public ICollection<Estado> Estados { get; set; } = new List<Estado>();
    }
}
