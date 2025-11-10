using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace dago.Models
{
    public class ConfiguracaoEsporadico
    {
        [Key]
        public int Id { get; set; }

        public List<ConfiguracaoEsporadicoCliente> ClientesExcluidos { get; set; } = new();
        public List<ConfiguracaoEsporadicoUnidade> UnidadesExcluidas { get; set; } = new();
        public List<ConfiguracaoEsporadicoDestinatario> DestinatariosExcluidos { get; set; } = new();
    }

    // ===============================
    // CLIENTES EXCLUÍDOS
    // ===============================
    public class ConfiguracaoEsporadicoCliente
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(ConfiguracaoEsporadico))]
        public int ConfiguracaoEsporadicoId { get; set; }
        public ConfiguracaoEsporadico ConfiguracaoEsporadico { get; set; } = null!;

        [ForeignKey(nameof(Cliente))]
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; } = null!;
    }

    // ===============================
    // UNIDADES EXCLUÍDAS
    // ===============================
    public class ConfiguracaoEsporadicoUnidade
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(ConfiguracaoEsporadico))]
        public int ConfiguracaoEsporadicoId { get; set; }
        public ConfiguracaoEsporadico ConfiguracaoEsporadico { get; set; } = null!;

        [ForeignKey(nameof(Unidade))]
        public int UnidadeId { get; set; }
        public Unidade Unidade { get; set; } = null!;
    }

    // ===============================
    // DESTINATÁRIOS EXCLUÍDOS
    // ===============================
    public class ConfiguracaoEsporadicoDestinatario
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(ConfiguracaoEsporadico))]
        public int ConfiguracaoEsporadicoId { get; set; }
        public ConfiguracaoEsporadico ConfiguracaoEsporadico { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string Destinatario { get; set; } = string.Empty;
    }
}
