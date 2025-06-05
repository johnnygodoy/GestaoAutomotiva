using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;

namespace GestaoAutomotiva.Utils
{
    public class RelatorioMultiplosGraficosPdf : IDocument
    {
        private readonly List<(string Titulo, byte[] Imagem)> _imagens;

        public RelatorioMultiplosGraficosPdf(List<(string Titulo, byte[] Imagem)> imagens) {
            _imagens = imagens;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container) {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().AlignCenter().Text("📊 PLANEJAMENTO DE ATIVIDADES - GRÁFICOS")
                    .Bold().FontSize(16).FontColor(Colors.Black);

                page.Content().Column(col =>
                {
                    foreach (var (titulo, imagem) in _imagens)
                    {
                        col.Item().ShowEntire().Column(box =>
                        {
                            // ✅ Correto: aplicar padding no container, não no Text()
                            box.Item().Element(e =>
                                e.PaddingBottom(5)
                                 .Text($"📊 {titulo}")
                                 .FontSize(13).Bold().FontColor("#003366")
                            );

                            box.Item()
                                .Border(1).BorderColor("#555").Padding(10).Background(Colors.White)
                                .Column(imgBox =>
                                {
                                    imgBox.Item().Image(imagem, ImageScaling.FitWidth);
                                });
                        });

                        col.Item().Height(10); // Espaço entre os blocos
                    }
                });


                page.Footer().AlignCenter().Text(txt =>
                {
                    txt.Span("Documento gerado em: ");
                    txt.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).SemiBold();
                });
            });
        }


    }


}

