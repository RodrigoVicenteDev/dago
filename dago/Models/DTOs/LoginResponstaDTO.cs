namespace dago.Models.DTOs
{
    public class LoginResponstaDTO
    {
        public string Token { get; set; } = string.Empty;

        public UsuarioResumo Usuario { get; set; } = new();

        public class UsuarioResumo
        {
            public int Id { get; set; }
            public string Nome { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? Cargo { get; set; }
            public List<ClienteResumo> Clientes { get; set; } = new();
        }
        public class ClienteResumo
        {
            public int Id { get; set; }
            public string Nome { get; set; } = string.Empty;
            public string Cnpj { get; set; } = string.Empty;
        }
    }
}
