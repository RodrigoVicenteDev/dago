using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dago.Models
{
    public class LeadTimeCliente
    {
        [Key]
        public int Id { get; set; }

        // Cliente
        [Required]
        [ForeignKey(nameof(Cliente))]
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; } = null!;

        // Cidade
        [Required]
        [ForeignKey(nameof(Cidade))]
        public int CidadeId { get; set; }
        public Cidade Cidade { get; set; } = null!;

        // Estado (redundância útil para query mais rápida)
        [Required]
        [ForeignKey(nameof(Estado))]
        public int EstadoId { get; set; }
        public Estado Estado { get; set; } = null!;

        // Dias de lead time
        [Required]
        public int DiasLead { get; set; }
    }
}
