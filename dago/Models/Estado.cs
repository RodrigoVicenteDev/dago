using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace dago.Models
{
    public class Estado
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CodigoUf { get; set; }

        [Required, StringLength(2)]
        public string Sigla { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        // Relacionamento 1:N → Cidades
        public ICollection<Cidade> Cidades { get; set; } = new List<Cidade>();
    }
}

