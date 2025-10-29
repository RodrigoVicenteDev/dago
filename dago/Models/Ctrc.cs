using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dago.Models
{
    
        public class Ctrc
        {
            [Key]
            public int Id { get; set; }

            [Required, MaxLength(50)]
            public string Numero { get; set; } = string.Empty;

            [Required]
            public DateTime DataEmissao { get; set; }

            [MaxLength(50)]
            public string? NumeroNotaFiscal { get; set; }

            [ForeignKey(nameof(Cliente))]
            public int ClienteId { get; set; }
            public Cliente Cliente { get; set; } = null!;

            [ForeignKey(nameof(CidadeDestino))]
            public int CidadeDestinoId { get; set; }
            public Cidade CidadeDestino { get; set; } = null!;

            [ForeignKey(nameof(EstadoDestino))]
            public int EstadoDestinoId { get; set; }
            public Estado EstadoDestino { get; set; } = null!;

            [ForeignKey(nameof(StatusEntrega))]
            public int StatusEntregaId { get; set; }
            public StatusEntrega StatusEntrega { get; set; } = null!;

            [Required]
            public int LeadTimeDias { get; set; }

            public DateTime? DataEntregaPrevista { get; set; }

            // Relacionamentos 1:N
            public ICollection<Agenda> Agendas { get; set; } = new List<Agenda>();
            public ICollection<OcorrenciaAtendimento> OcorrenciasAtendimento { get; set; } = new List<OcorrenciaAtendimento>();
            public ICollection<OcorrenciaSistema> OcorrenciasSistema { get; set; } = new List<OcorrenciaSistema>();
            public ParticularidadeCliente? ParticularidadeCliente { get; set; }
        }
    }

