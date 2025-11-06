using System.Collections.Generic;

namespace dago.Models.DTOs
{
    public class ImportCacheData
    {
        public List<object> LinhasValidas { get; set; } = new();
        public List<LinhaErroImportacaoDTO> LinhasComErro { get; set; } = new();
    }
}
