using dago.Models.DTOs;
using System.Globalization;
using CsvHelper.Configuration;


namespace dago.CsvMaps
{
    public sealed class CtrcCsvRowMap : ClassMap<CtrcCsvRow>
    {
        public CtrcCsvRowMap()
        {
            Map(m => m.SerieNumeroCtrc).Name("Serie/Numero CTRC");
            Map(m => m.TipoDocumento).Name("Tipo do Documento");
            Map(m => m.DataEmissao).Name("Data de Emissao")
                .TypeConverterOption.Format("dd/MM/yyyy")
                .TypeConverterOption.CultureInfo(new CultureInfo("pt-BR"));
            Map(m => m.PracaExpedidora).Name("Praca Expedidora");
            Map(m => m.ClienteRemetente).Name("Cliente Remetente");
            Map(m => m.ClienteDestinatario).Name("Cliente Destinatario");
            Map(m => m.CidadeEntrega).Name("Cidade de Entrega");
            Map(m => m.UfEntrega).Name("UF de Entrega");
            Map(m => m.UnidadeReceptora).Name("Unidade Receptora");
            Map(m => m.NumeroNotaFiscal).Name("Numero da Nota Fiscal");
            Map(m => m.DataUltimaOcorrencia).Name("Data da Ultima Ocorrencia");
            Map(m => m.DescricaoUltimaOcorrencia).Name("Descricao da Ultima Ocorrencia");
            Map(m => m.DataEntregaRealizada).Name("Data da Entrega Realizada");
            Map(m => m.NotasFiscais).Name("Notas Fiscais");
            Map(m => m.PesoCalculadoKg).Name("Peso Calculado em Kg");
        }
    }
}
