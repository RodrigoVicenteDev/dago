using System.ComponentModel.DataAnnotations;

namespace dago.Models.DTOs
{
    public class AtualizarUsuarioDTO
    {
        
        public string?  Nome { get; set; }
        public string? Email { get; set; }
        public string? NovaSenha { get; set; }
        public int? CargoId { get; set; }

        public List<int>? ClientesIds { get; set; }


    }
}
