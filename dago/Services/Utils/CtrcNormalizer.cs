using dago.Data;
using dago.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace dago.Services.Utils
{
    public class CtrcNormalizer
    {
        private readonly AppDbContext _db;

        public CtrcNormalizer(AppDbContext db)
        {
            _db = db;
        }

        // ============================================================
        // NORMALIZAÇÃO ROBUSTA (IGNORA ACENTOS / Ç / CASE / EXTRAS)
        // ============================================================
        public static string Normalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Remove múltiplos espaços
            string text = Regex.Replace(input.Trim(), @"\s+", " ");

            // Remove acentos
            var sb = new StringBuilder();
            foreach (char c in text.Normalize(NormalizationForm.FormD))
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            string semAcentos = sb.ToString().Normalize(NormalizationForm.FormC);

            // ç -> c
            semAcentos = semAcentos.Replace('ç', 'c').Replace('Ç', 'C');

            // Maiúsculas
            semAcentos = semAcentos.ToUpperInvariant();

            // Remove lixo invisível mas mantém ' e -
            semAcentos = Regex.Replace(semAcentos, @"[^A-Z0-9' \-]", "");

            return semAcentos;
        }

        public static bool EqualsNormalized(string a, string b)
            => Normalize(a) == Normalize(b);


        // ============================================================
        // R E S O L V E   C L I E N T E ,   C I D A D E ,   E S T A D O
        // ============================================================
        public async Task<(Cliente cliente, Cidade cidade, Estado estado, Unidade unidade)> ResolverAsync(
            string nomeCliente,
            string nomeDestinatario,
            string cidadeEntrega,
            string ufEntrega,
            string unidadeReceptora)
        {
            // ------------------------------------------------------------
            // ESTADO (por sigla normalizada)
            // ------------------------------------------------------------
            string ufNorm = Normalize(ufEntrega);

            var estados = await _db.Estados.ToListAsync();
            var estado = estados.FirstOrDefault(e => Normalize(e.Sigla) == ufNorm);

            if (estado == null)
            {
                estado = new Estado
                {
                    Sigla = ufEntrega.ToUpper(),
                    Nome = ufEntrega.ToUpper(),
                    CodigoUf = 0,
                    RegiaoEstadoId = 1
                };
                _db.Estados.Add(estado);
                await _db.SaveChangesAsync();
            }


            // ------------------------------------------------------------
            // CIDADE (por nome + estado, normalizado)
            // ------------------------------------------------------------
            var cidadesDoEstado = await _db.Cidades
                .Where(c => c.EstadoId == estado.Id)
                .ToListAsync();

            var cidade = cidadesDoEstado
                .FirstOrDefault(c => EqualsNormalized(c.Nome, cidadeEntrega));

            if (cidade == null)
            {
                cidade = new Cidade
                {
                    Nome = cidadeEntrega.ToUpper(),
                    EstadoId = estado.Id,
                    TipoRegiaoId = 1
                };
                _db.Cidades.Add(cidade);
                await _db.SaveChangesAsync();
            }


            // ------------------------------------------------------------
            // UNIDADE (normalizada)
            // ------------------------------------------------------------
            var unidades = await _db.Unidades.ToListAsync();

            var unidade = unidades
                .FirstOrDefault(u => EqualsNormalized(u.Nome, unidadeReceptora));

            if (unidade == null)
            {
                unidade = new Unidade
                {
                    Nome = unidadeReceptora.ToUpper(),
                    EstadoId = estado.Id
                };
                _db.Unidades.Add(unidade);
                await _db.SaveChangesAsync();
            }


            // ------------------------------------------------------------
            // CLIENTE REMETENTE (normalizado)
            // ------------------------------------------------------------
            var clientes = await _db.Clientes.ToListAsync();

            var cliente = clientes
                .FirstOrDefault(c => EqualsNormalized(c.Nome, nomeCliente));

            if (cliente == null)
            {
                cliente = new Cliente
                {
                    Nome = nomeCliente.ToUpper(),
                    Cnpj = Guid.NewGuid().ToString()
                };
                _db.Clientes.Add(cliente);
                await _db.SaveChangesAsync();
            }

            return (cliente, cidade, estado, unidade);
        }
    }
}
