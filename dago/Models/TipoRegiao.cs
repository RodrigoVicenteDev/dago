using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace dago.Models
{
    public class TipoRegiao
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Nome { get; set; } = string.Empty;

        // Relacionamento 1:N → Cidades
        public ICollection<Cidade> Cidades { get; set; } = new List<Cidade>();
    }
}
