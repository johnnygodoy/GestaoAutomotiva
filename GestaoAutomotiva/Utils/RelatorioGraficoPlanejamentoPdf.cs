using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;

namespace GestaoAutomotiva.Utils
{
    public class RelatorioPlanejamentoPdf : IDocument
    {
        private readonly byte[] _imagemGrafico;

        public RelatorioPlanejamentoPdf(byte[] imagemGrafico) {
            _imagemGrafico = imagemGrafico;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container) {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Content().Column(col =>
                {
                    // ✅ Título com espaçamento
                    col.Item().PaddingBottom(10).AlignCenter().Text("📊 PLANEJAMENTO DE ATIVIDADES - GRÁFICO DE CONFLITOS")
                        .Bold().FontSize(16).FontColor(Colors.Black);

                    // ✅ Caixa com gráfico
                    col.Item().Border(1).BorderColor("#555").Padding(10).Background(Colors.White).Column(box =>
                    {
                        box.Item().Image(_imagemGrafico, ImageScaling.FitWidth);
                    });

                    // ✅ Rodapé com data
                    col.Item().PaddingTop(20).AlignCenter().Text(txt =>
                    {
                        txt.Span("Documento gerado em: ");
                        txt.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).SemiBold();
                    });
                });
            });
        }


    }
}
