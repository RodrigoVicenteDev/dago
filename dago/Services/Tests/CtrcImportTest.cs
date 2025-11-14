using dago.Data;
using dago.Services.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dago.Tests
{
    public static class CtrcImportTest
    {
        public static async Task RunAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            using var transaction = await db.Database.BeginTransactionAsync();

            Console.WriteLine();
            Console.WriteLine("=========================================================");
            Console.WriteLine("🔍 TESTE COMPLETO DE NORMALIZAÇÃO (sem alterar o banco)");
            Console.WriteLine("=========================================================\n");

            var normalizer = new CtrcNormalizer(db);

            // ==========================================================
            // 1) LISTA DE VARIAÇÕES PARA TESTAR
            // ==========================================================

            var cidadesParaTestar = new[]
            {
                "ARACAJU",
                "ARACAJÚ",
                "Aracaju",
                "A R A C A J U",
                "aracaju",
                "ARACAJU   ",
                " ARACAJÚ ",
            };

            var clientesParaTestar = new[]
            {
                "HENKEL LTDA",
                "Henkel Ltda",
                "henkel ltda",
                "HÉNKÉL LTDA",
                "HENKEL     LTDA",
                "h e n k e l   l t d a"
            };

            var unidadesParaTestar = new[]
            {
                "AJU",
                "Aju",
                "aju",
                "A J U",
                "AJÚ",
                "A-J-U"
            };

            var compostosParaTestar = new[]
            {
                "D'OESTE",
                "D’OESTE",
                "D OESTE",
                "DOESTE",
                "D- OESTE",
                "D O É S T E",
                "d'oeste"
            };

            // ==========================================================
            // 2) FUNÇÃO INTERNA PARA RODAR CADA TESTE
            // ==========================================================
            async Task TestarGrupo(string titulo, IEnumerable<string> valores, Func<string, Task<(bool found, string? nomeBanco)>> resolver)
            {
                Console.WriteLine($"-----------------------------");
                Console.WriteLine($"🔎 Testando {titulo}");
                Console.WriteLine($"-----------------------------");

                foreach (var v in valores)
                {
                    string norm = CtrcNormalizer.Normalize(v);

                    var (found, nomeBanco) = await resolver(v);

                    Console.WriteLine(
                        $"Entrada: \"{v}\" | Normalizado: \"{norm}\" | Encontrado: {(found ? "SIM" : "NÃO")} {(found ? $"→ \"{nomeBanco}\"" : "")}"
                    );
                }

                Console.WriteLine();
            }

            // ==========================================================
            // 3) TESTA CIDADES
            // ==========================================================
            await TestarGrupo("Cidades", cidadesParaTestar, async (entrada) =>
            {
                var todas = await db.Cidades.ToListAsync();
                var item = todas.FirstOrDefault(c => CtrcNormalizer.EqualsNormalized(c.Nome, entrada));
                return (item != null, item?.Nome);
            });

            // ==========================================================
            // 4) TESTA CLIENTES
            // ==========================================================
            await TestarGrupo("Clientes", clientesParaTestar, async (entrada) =>
            {
                var todos = await db.Clientes.ToListAsync();
                var item = todos.FirstOrDefault(c => CtrcNormalizer.EqualsNormalized(c.Nome, entrada));
                return (item != null, item?.Nome);
            });

            // ==========================================================
            // 5) TESTA UNIDADES
            // ==========================================================
            await TestarGrupo("Unidades", unidadesParaTestar, async (entrada) =>
            {
                var todos = await db.Unidades.ToListAsync();
                var item = todos.FirstOrDefault(u => CtrcNormalizer.EqualsNormalized(u.Nome, entrada));
                return (item != null, item?.Nome);
            });

            // ==========================================================
            // 6) TESTA NOMES COMPOSTOS (D'OESTE)
            // ==========================================================
            await TestarGrupo("Nomes compostos (D'OESTE)", compostosParaTestar, async (entrada) =>
            {
                var todos = await db.Cidades.ToListAsync();

                var item = todos.FirstOrDefault(c =>
                    CtrcNormalizer.EqualsNormalized(c.Nome, entrada)
                );

                return (item != null, item?.Nome);
            });

            // ==========================================================
            // FINALIZAÇÃO
            // ==========================================================
            await transaction.RollbackAsync();

            Console.WriteLine("=========================================================");
            Console.WriteLine("🔄 Teste finalizado — rollback executado (banco intacto)");
            Console.WriteLine("=========================================================\n");
        }
    }
}
