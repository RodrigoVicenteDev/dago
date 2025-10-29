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
        [ForeignKey(nameof(TipoOcorrencia))]
        public int TipoOcorrenciaId { get; set; }
        public TipoOcorrencia TipoOcorrencia { get; set; } = null!;

        [Required]
        public DateTime Data { get; set; }

        [MaxLength(300)]
        public string? Descricao { get; set; }
    }
}
