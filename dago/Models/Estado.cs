using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace dago.Models
{
    public class Estado
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CodigoUf { get; set; }

        [Required, MaxLength(2)]
        public string Sigla { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        // 🔹 FK para RegiaoEstado
        [ForeignKey(nameof(RegiaoEstado))]
        public int RegiaoEstadoId { get; set; }
        public RegiaoEstado RegiaoEstado { get; set; } = null!;

        // Relacionamentos
        public ICollection<Cidade> Cidades { get; set; } = new List<Cidade>();
        public ICollection<Unidade> Unidades { get; set; } = new List<Unidade>();
        public ICollection<Ctrc> Ctrcs { get; set; } = new List<Ctrc>();
    }
}

