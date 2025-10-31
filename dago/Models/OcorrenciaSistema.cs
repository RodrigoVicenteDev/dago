using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dago.Models
{
    public class OcorrenciaSistema
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Ctrc))]
        public int CtrcId { get; set; }
        public Ctrc Ctrc { get; set; } = null!;
        [Required]
        public int NumeroOcorrencia { get; set; }

        [Required]
        public DateTime Data { get; set; }

        [MaxLength(300)]
        public string? Descricao { get; set; }
    }
}
