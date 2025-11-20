using CsvHelper.Configuration;
using dago.Models.DTOs;

namespace dago.CsvMaps
{
    public sealed class CtrcCsvRowMap : ClassMap<CtrcCsvRow>
    {
        public CtrcCsvRowMap()
        {
            Map(m => m.SerieNumeroCtrc).Name("CTRC", "SerieNumeroCtrc");
            Map(m => m.TipoDocumento).Name("TipoDocumento", "Tipo");
            Map(m => m.DataEmissao).Name("DataEmissao");
            Map(m => m.PracaExpedidora).Name("PracaExpedidora");

            Map(m => m.ClienteRemetente).Name("ClienteRemetente", "Remetente");

            // ⭐ ⭐ ⭐ ESTE ERA O CAMPO FALTANDO ⭐ ⭐ ⭐
            Map(m => m.CnpjRemetente)
                .Name("CNPJ Remetente", "CNPJ_Remetente", "CNPJRemetente", "CNPJ", "Remetente CNPJ");

            Map(m => m.ClienteDestinatario).Name("ClienteDestinatario", "Destinatario");
            Map(m => m.CidadeEntrega).Name("CidadeEntrega", "Cidade");
            Map(m => m.UfEntrega).Name("UfEntrega", "UF");
            Map(m => m.UnidadeReceptora).Name("UnidadeReceptora", "Unidade");
            Map(m => m.NumeroNotaFiscal).Name("NumeroNotaFiscal", "NF");
            Map(m => m.DataUltimaOcorrencia).Name("DataUltimaOcorrencia");
            Map(m => m.DescricaoUltimaOcorrencia).Name("DescricaoUltimaOcorrencia");
            Map(m => m.DataEntregaRealizada).Name("DataEntregaRealizada");
            Map(m => m.NotasFiscais).Name("NotasFiscais");
            Map(m => m.PesoCalculadoKg).Name("PesoCalculadoKg", "PesoKg");
        }
    }
}
