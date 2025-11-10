using System;
using System.Threading.Tasks;
using dago.Data;
using dago.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace dago.Services.Tests
{
    public static class DateValidationTest
    {
        public static async Task RunAsync(IServiceProvider provider)
        {
            using var scope = provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            Console.WriteLine("\n═══════════════════════════════════════════");
            Console.WriteLine("🧪 INICIANDO TESTE DE DATAS PURAS (SEM FUSO)");
            Console.WriteLine("═══════════════════════════════════════════\n");

           
            // 1️⃣ Cria CTRC temporário com hora e UTC simuladas
            var ctrc = new Ctrc
            {
                Numero = "TESTE-DATA-" + Guid.NewGuid().ToString("N")[..6],
                ClienteId = 4,
                CidadeDestinoId = 1,
                EstadoDestinoId = 1,
                UnidadeId = 1,
                DataEmissao = DateTime.UtcNow,
                DataPrevistaEntrega = DateTime.UtcNow.AddDays(5),
                DataEntregaRealizada = DateTime.UtcNow.AddDays(7),
                Peso = 1.5m,
                LeadTimeDias = 5,
                StatusEntregaId = 1
            };

            db.Ctrcs.Add(ctrc);
            await db.SaveChangesAsync();

            // 2️⃣ Recarrega e imprime resultados
            var reloaded = await db.Ctrcs.AsNoTracking().FirstAsync(x => x.Id == ctrc.Id);

            Console.WriteLine($"📦 CTRC salvo: {reloaded.Numero}");
            Console.WriteLine($"🗓 DataEmissao ..............: {reloaded.DataEmissao:yyyy-MM-dd HH:mm:ss} ({reloaded.DataEmissao.Kind})");
            Console.WriteLine($"🗓 DataPrevistaEntrega ......: {reloaded.DataPrevistaEntrega:yyyy-MM-dd HH:mm:ss} ({reloaded.DataPrevistaEntrega?.Kind})");
            Console.WriteLine($"🗓 DataEntregaRealizada .....: {reloaded.DataEntregaRealizada:yyyy-MM-dd HH:mm:ss} ({reloaded.DataEntregaRealizada?.Kind})");

            var diff = (reloaded.DataEntregaRealizada!.Value - reloaded.DataEmissao).Days;
            Console.WriteLine($"📏 Diferença em dias ........: {diff} dias");

            Console.WriteLine("\n✅ Teste concluído com sucesso!\n");
        }
    }
}
