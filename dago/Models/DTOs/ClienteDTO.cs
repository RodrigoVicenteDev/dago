namespace dago.Models.DTOs
{
    public class ClienteDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;

        // opcional: incluir info resumida do usuário responsável
        public UsuarioResumoDTO? Usuario { get; set; }
    }

    public class UsuarioResumoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
       
    }
}
