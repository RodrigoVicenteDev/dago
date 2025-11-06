using dago.Data;
using dago.Models;
using Microsoft.EntityFrameworkCore;

namespace dago.Services.Utils
{
    public class CtrcNormalizer
    {
        private readonly AppDbContext _db;

        public CtrcNormalizer(AppDbContext db)
        {
            _db = db;
        }

        // 🔹 Resolve ou cria cliente, cidade, estado e unidade
        public async Task<(Cliente cliente, Cidade cidade, Estado estado, Unidade unidade)> ResolverAsync(
            string nomeCliente,
            string nomeDestinatario,
            string cidadeEntrega,
            string ufEntrega,
            string unidadeReceptora)
        {
            // 🔸 Estado
            var estado = await _db.Estados
                .FirstOrDefaultAsync(e => e.Sigla.ToUpper() == ufEntrega.ToUpper());

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

            // 🔸 Cidade
            var cidade = await _db.Cidades
                .FirstOrDefaultAsync(c =>
                    c.Nome.ToUpper() == cidadeEntrega.ToUpper() &&
                    c.EstadoId == estado.Id);

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

            // 🔸 Unidade
            var unidade = await _db.Unidades
                .FirstOrDefaultAsync(u => u.Nome.ToUpper() == unidadeReceptora.ToUpper());

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

            // 🔸 Cliente remetente
            var cliente = await _db.Clientes
                .FirstOrDefaultAsync(c => c.Nome.ToUpper() == nomeCliente.ToUpper());

            if (cliente == null)
            {
                cliente = new Cliente
                {
                    Nome = nomeCliente.ToUpper(),
                    Cnpj = Guid.NewGuid().ToString() // placeholder
                };
                _db.Clientes.Add(cliente);
                await _db.SaveChangesAsync();
            }

            // 🔹 Tudo pronto
            return (cliente, cidade, estado, unidade);
        }
    }
}
