using dago.Data;
using dago.Models;
using dago.Services.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace dago.Tests
{
    public static class TesteNormalizerCnpj
    {
        public static async Task RunAsync(IServiceProvider services)
        {
            Console.WriteLine("\n==============================================");
            Console.WriteLine("🔥 EXECUTANDO TESTE DE NORMALIZAÇÃO POR CNPJ 🔥");
            Console.WriteLine("==============================================\n");

            using var scope = services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var normalizer = scope.ServiceProvider.GetRequiredService<CtrcNormalizer>();

            // ================================
            // CLIENTES DE TESTE
            // ================================
            var cliente1 = new Cliente { Nome = "CNA S.A.", Cnpj = "60881299000677" };
            var cliente2 = new Cliente { Nome = "CNA S/A", Cnpj = "60881299000910" };

            // =====================================================
            // CORREÇÃO:
            // NÃO DELETAR CLIENTES QUE POSSAM TER CTRCs VINCULADOS
            // =====================================================

            

            Console.WriteLine("Clientes testados:");
            Console.WriteLine($"➡ {cliente1.Nome} - {cliente1.Cnpj}");
            Console.WriteLine($"➡ {cliente2.Nome} - {cliente2.Cnpj}");

            // ================================
            // TESTE 1: CNA S.A.
            // ================================
            Console.WriteLine("\n🔍 Teste 1 – Buscar por 60881299000677");

            var (cliA, cidadeA, estadoA, unidadeA) = await normalizer.ResolverAsync(
                nomeCliente: cliente1.Nome,
                cnpjCliente: cliente1.Cnpj,
                nomeDestinatario: "DEST TESTE",
                cidadeEntrega: "SAO PAULO",
                ufEntrega: "SP",
                unidadeReceptora: "GRU"
            );

            Console.WriteLine($"Encontrado: {cliA?.Nome} - {cliA?.Cnpj}");

            if (cliA?.Cnpj != cliente1.Cnpj)
                Console.WriteLine("❌ ERRO: cliente 1 retornado incorretamente!");
            else
                Console.WriteLine("✔ SUCESSO: cliente 1 OK");


            // ================================
            // TESTE 2: CNA S/A.
            // ================================
            Console.WriteLine("\n🔍 Teste 2 – Buscar por 60881299000910");

            var (cliB, cidadeB, estadoB, unidadeB) = await normalizer.ResolverAsync(
                nomeCliente: cliente2.Nome,
                cnpjCliente: cliente2.Cnpj,
                nomeDestinatario: "DEST TESTE",
                cidadeEntrega: "SAO PAULO",
                ufEntrega: "SP",
                unidadeReceptora: "GRU"
            );

            Console.WriteLine($"Encontrado: {cliB?.Nome} - {cliB?.Cnpj}");

            if (cliB?.Cnpj != cliente2.Cnpj)
                Console.WriteLine("❌ ERRO: cliente 2 retornado incorretamente!");
            else
                Console.WriteLine("✔ SUCESSO: cliente 2 OK");

            // ================================
            // LIMPEZA
            // ================================
            Console.WriteLine("\n🧹 Limpando dados...");

           

            Console.WriteLine("✔ Teste finalizado com sucesso!");
        }
    }
}
