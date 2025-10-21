using System.ComponentModel.DataAnnotations;

namespace dago.Models.DTOs
{
    public class CadastrarCargoDTO
    {
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

       
    }
}
