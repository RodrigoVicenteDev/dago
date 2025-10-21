using System.ComponentModel.DataAnnotations;

namespace dago.Models;

public class Usuario
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Nome { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Senha { get; set; }
    [Required]
    public int CargoId { get; set; }

    [Required]
    public Cargo Cargo { get; set; } = null!;


    public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

}
