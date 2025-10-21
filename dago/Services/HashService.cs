using Microsoft.AspNetCore.Identity;

namespace dago.Services
{
    public class HashService
    {
        private readonly PasswordHasher<object> _hasher = new();

        public string GerarHash(string senhaEmTexto)
        {
            // Retorna string contendo algoritmo/versão + salt + hash (formato próprio do ASP.NET)
            return _hasher.HashPassword(user: null, password: senhaEmTexto);
        }


        public bool Verificar(string senhaEmTexto, string hashArmazenado)
        {
            var result = _hasher.VerifyHashedPassword(user: null, hashedPassword: hashArmazenado, providedPassword: senhaEmTexto);
            return result == PasswordVerificationResult.Success ||
                   result == PasswordVerificationResult.SuccessRehashNeeded; // permite login e você pode re-hash depois
        }
    }
}
