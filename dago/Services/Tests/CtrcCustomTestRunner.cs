using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public static class CtrcCustomTestRunner
{
    public static async Task RunAsync(IServiceProvider services)
    {
        Console.WriteLine("\n================ TESTES CTRC =================\n");

        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<dago.Data.AppDbContext>();

        var tests = new Tests.CtrcCustomTests(db);

        await tests.TesteOcorrenciaDiasAsync();
        await tests.TesteCsvNaoApagaDataEntregaDigitadaAsync();
        await tests.TesteDiasEntreOcorrencias3Async();

        Console.WriteLine("\n=========== TODOS OS TESTES PASSARAM ✔ ==========\n");
    }
}
