using System.ComponentModel.DataAnnotations;

namespace dago.Models.DTOs
{
    public class CadastrarUsuarioDTO
    {
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        [MaxLength(50)]
        public string Senha { get; set; } = string.Empty;

        [Required]
        public int CargoId { get; set; }

        public List<int>? ClientesID { get; set; }
    }
}
