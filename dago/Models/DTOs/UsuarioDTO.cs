namespace dago.Models.DTOs
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public CargoDTO? Cargo { get; set; } 

        public List<ClienteDTO>? Clientes { get; set; } 
    }
}
