using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dago.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        public string Cnpj { get; set; }

        [ForeignKey(nameof(Usuario))]
        public int? UsuarioId { get; set; }

       
        public Usuario? Usuario { get; set; } 
    }
}
