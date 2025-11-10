using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace dago.Models
{
    /// <summary>
    /// Define as regras globais para filtragem de CTRCs esporádicos.
    /// O gerente pode configurar listas de clientes, unidades e destinatários
    /// que NÃO devem aparecer na visão dos usuários esporádicos.
    /// </summary>
    public class ConfiguracaoEsporadico
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Clientes que NÃO fazem parte dos CTRCs esporádicos.
        /// </summary>
        public List<ConfiguracaoEsporadicoCliente> ClientesExcluidos { get; set; } = new();

        /// <summary>
        /// Unidades que NÃO fazem parte dos CTRCs esporádicos.
        /// </summary>
        public List<ConfiguracaoEsporadicoUnidade> UnidadesExcluidas { get; set; } = new();

        /// <summary>
        /// Destinatários (texto) que NÃO fazem parte dos CTRCs esporádicos.
        /// </summary>
        public List<ConfiguracaoEsporadicoDestinatario> DestinatariosExcluidos { get; set; } = new();
    }

    // 👇 Subentidades simples (relacionadas à principal)

    public class ConfiguracaoEsporadicoCliente
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Cliente))]
        public int ClienteId { get; set; }

        public Cliente Cliente { get; set; } = null!;
    }

    public class ConfiguracaoEsporadicoUnidade
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Unidade))]
        public int UnidadeId { get; set; }

        public Unidade Unidade { get; set; } = null!;
    }

    public class ConfiguracaoEsporadicoDestinatario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Destinatario { get; set; } = string.Empty;
    }
}
