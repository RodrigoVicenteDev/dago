using System.Collections.Generic;

namespace dago.Models.DTOs
{
    public class ConfiguracaoEsporadicoDTO
    {
        public List<int> ClientesExcluidos { get; set; } = new();
        public List<int> UnidadesExcluidas { get; set; } = new();
        public List<string> DestinatariosExcluidos { get; set; } = new();
    }
}
