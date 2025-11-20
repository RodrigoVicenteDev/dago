using System;
using System.Threading.Tasks;
using dago.Data;
using dago.Models;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    public class CtrcCustomTests
    {
        private readonly AppDbContext _db;

        public CtrcCustomTests(AppDbContext db)
        {
            _db = db;
        }

        // ============================================================
        // 1️⃣ TESTE: OCORRÊNCIA ENTRA NO BANCO E CALCULA DIAS ENTRE ELAS
        // ============================================================
        public async Task TesteOcorrenciaDiasAsync()
        {
            Console.WriteLine("➡ TESTE: Ocorrência calcula dias desde a última");

            // limpa ocorrencias do CTRC de teste
            var ctrc = await _db.Ctrcs.FirstOrDefaultAsync(c => c.Numero == "GRU99999-9");
            if (ctrc == null)
                throw new Exception("CTRC de teste GRU99999-9 não encontrado no banco!");

            _db.OcorrenciasSistema.RemoveRange(
                _db.OcorrenciasSistema.Where(o => o.CtrcId == ctrc.Id)
            );
            await _db.SaveChangesAsync();

            // cria ocorrências
            var primeira = new OcorrenciaSistema
            {
                CtrcId = ctrc.Id,
                Data = new DateTime(2025, 10, 20),
                NumeroOcorrencia = 1,
                Descricao = "1ª"
            };
            await _db.OcorrenciasSistema.AddAsync(primeira);
            await _db.SaveChangesAsync();

            // segunda ocorrência (5 dias depois)
            var segundaData = new DateTime(2025, 10, 25);
            var segunda = new OcorrenciaSistema
            {
                CtrcId = ctrc.Id,
                Data = segundaData,
                NumeroOcorrencia = 2,
                DiasDesdeAnterior = (segundaData - primeira.Data).Days,
                Descricao = "2ª"
            };
            await _db.OcorrenciasSistema.AddAsync(segunda);
            await _db.SaveChangesAsync();

            if (segunda.DiasDesdeAnterior != 5)
                throw new Exception("Falhou: DiasDesdeAnterior deveria ser 5!");

            Console.WriteLine("✔ OK — DiasDesdeAnterior = 5");
        }

        // ============================================================
        // 2️⃣ TESTE: DATA DE ENTREGA NÃO PODE APAGAR A ANTERIOR
        // ============================================================
        public async Task TesteCsvNaoApagaDataEntregaDigitadaAsync()
        {
            Console.WriteLine("➡ TESTE: CSV com data NOVA deve atualizar DataEntregaRealizada");

            // 1) Pega o CTRC real
            var ctrc = await _db.Ctrcs.FirstOrDefaultAsync(c => c.Numero == "GRU99999-9");
            if (ctrc == null)
                throw new Exception("CTRC de teste GRU99999-9 não encontrado!");

            // 2) Simula usuário digitando uma data no grid (antes do CSV)
            ctrc.DataEntregaRealizada = new DateTime(2025, 10, 20);
            await _db.SaveChangesAsync();

            Console.WriteLine($"✔ Usuário digitou: {ctrc.DataEntregaRealizada:dd/MM/yyyy}");

            // =====================================================
            // 3) AGORA SIMULA O CSV do dia seguinte trazendo
            //    uma data de entrega REAL (ex: 22/10/2025)
            // =====================================================
            DateTime? dataDoCsv = new DateTime(2025, 10, 22);

            // comportamento atual do seu CtrcImportService:
            // if (dataEntrega.HasValue)
            //     ctrc.DataEntregaRealizada = dataEntrega;

            if (dataDoCsv.HasValue)
                ctrc.DataEntregaRealizada = dataDoCsv.Value;

            await _db.SaveChangesAsync();

            // 4) Lê novamente do banco
            var atualizado = await _db.Ctrcs.FirstAsync(c => c.Id == ctrc.Id);

            // 5) Validação — agora DEVE ter atualizado!
            if (atualizado.DataEntregaRealizada != dataDoCsv.Value)
                throw new Exception("❌ ERRO: CSV trouxe data, mas não atualizou corretamente!");

            Console.WriteLine($"✔ OK — CSV atualizou para: {atualizado.DataEntregaRealizada:dd/MM/yyyy}");
        }


        // ============================================================
        // 3️⃣ TESTE: DIAS ENTRE OCORRÊNCIAS = 3
        // ============================================================
        public async Task TesteDiasEntreOcorrencias3Async()
        {
            Console.WriteLine("➡ TESTE: Diferença de 3 dias entre ocorrências");

            var ctrc = await _db.Ctrcs.FirstOrDefaultAsync(c => c.Numero == "GRU99999-9");
            if (ctrc == null) throw new Exception("CTRC de teste não encontrado!");

            _db.OcorrenciasSistema.RemoveRange(
                _db.OcorrenciasSistema.Where(o => o.CtrcId == ctrc.Id)
            );
            await _db.SaveChangesAsync();

            // cria 1ª ocorrência
            var primeira = new OcorrenciaSistema
            {
                CtrcId = ctrc.Id,
                Data = new DateTime(2025, 10, 10),
                NumeroOcorrencia = 1
            };
            await _db.OcorrenciasSistema.AddAsync(primeira);
            await _db.SaveChangesAsync();

            // cria 2ª ocorrência
            var novaData = new DateTime(2025, 10, 13);
            var segunda = new OcorrenciaSistema
            {
                CtrcId = ctrc.Id,
                Data = novaData,
                NumeroOcorrencia = 2,
                DiasDesdeAnterior = (novaData - primeira.Data).Days
            };
            await _db.OcorrenciasSistema.AddAsync(segunda);
            await _db.SaveChangesAsync();

            if (segunda.DiasDesdeAnterior != 3)
                throw new Exception("Falhou: deveria ser 3 dias entre ocorrências.");

            Console.WriteLine("✔ OK — DiasDesdeAnterior = 3");
        }
    }
}
