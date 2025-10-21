using System.ComponentModel.DataAnnotations;

namespace dago.Models.DTOs
{
    public class CadastrarClienteDTO
    {
        [Required]
        public string Nome { get; set; }
        [Required]
        public string Cnpj { get; set; }
        public int? UsuarioId { get; set; }

    }
}
