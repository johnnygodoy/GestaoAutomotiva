using GestaoAutomotiva.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;

namespace GestaoAutomotiva.Utils
{
    public class RelatorioAtividadePdf : IDocument
    {
        private readonly List<Atividade> _atividades;

        public RelatorioAtividadePdf(List<Atividade> atividades) {
            _atividades = atividades;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container) {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4.Landscape());
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Element(e =>
                {
                    e.Container().PaddingBottom(15).AlignCenter().Text("Histórico de Atividades")
                     .Bold()
                     .FontSize(18);
                });

                page.Content().AlignCenter().Element(content =>
                {
                    content.Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(); // Funcionário
                            columns.RelativeColumn(); // Serviço
                            columns.ConstantColumn(60);  // Código do Carro
                            columns.RelativeColumn(); // Modelo
                            columns.ConstantColumn(80);  // Início
                            columns.ConstantColumn(80);  // Previsão
                            columns.ConstantColumn(80);  // Status
                        });

                        // Cabeçalho com fundo e bold
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Funcionário").Bold();
                            header.Cell().Element(CellStyle).Text("Serviço").Bold();
                            header.Cell().Element(CellStyle).Text("Código do Carro").Bold();
                            header.Cell().Element(CellStyle).Text("Modelo").Bold();
                            header.Cell().Element(CellStyle).Text("Início").Bold();
                            header.Cell().Element(CellStyle).Text("Previsão").Bold();
                            header.Cell().Element(CellStyle).Text("Status").Bold();
                        });

                        foreach (var a in _atividades)
                        {
                            table.Cell().Element(CellStyle).Text(a.Funcionario?.Nome ?? "-");
                            table.Cell().Element(CellStyle).Text(a.Servico?.Descricao ?? "-");
                            table.Cell().Element(CellStyle).Text(a.Carro?.IdCarro ?? "-");
                            table.Cell().Element(CellStyle).Text(a.Carro?.Modelo.Nome ?? "-");
                            table.Cell().Element(CellStyle).Text(a.DataInicio?.ToString("dd/MM/yyyy") ?? "-");
                            table.Cell().Element(CellStyle).Text(a.DataPrevista?.ToString("dd/MM/yyyy") ?? "-");
                            table.Cell().Element(CellStyle).Text(a.Status ?? "-");
                        }
                    });
                });

                page.Footer().AlignCenter().Text(txt =>
                {
                    txt.Span("Documento gerado por GestãoAutomotiva ");
                    txt.Span(DateTime.Now.ToString("dd/MM/yyyy")).SemiBold();
                });
            });
        }

        // Função para aplicar padding em todas as células
        static IContainer CellStyle(IContainer container) {
            return container.PaddingVertical(5).PaddingHorizontal(5);
        }

    }
}
