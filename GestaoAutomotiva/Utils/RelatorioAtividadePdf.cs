using GestaoAutomotiva.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;

namespace GestaoAutomotiva.Utils
{
    public class RelatorioAtividadePdf : IDocument
    {
        private readonly List<AtividadeHistorico> _historico;

        public RelatorioAtividadePdf(List<AtividadeHistorico> historico) {
            _historico = historico;
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
                            columns.ConstantColumn(80);  // Ação
                            columns.ConstantColumn(100); // Data Registro
                        });

                        // Cabeçalho
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Funcionário").Bold();
                            header.Cell().Element(CellStyle).Text("Serviço").Bold();
                            header.Cell().Element(CellStyle).Text("Código do Carro").Bold();
                            header.Cell().Element(CellStyle).Text("Modelo").Bold();
                            header.Cell().Element(CellStyle).Text("Início").Bold();
                            header.Cell().Element(CellStyle).Text("Previsão").Bold();
                            header.Cell().Element(CellStyle).Text("Status").Bold();
                            header.Cell().Element(CellStyle).Text("Ação").Bold();
                            header.Cell().Element(CellStyle).Text("Registro").Bold();
                        });

                        foreach (var h in _historico)
                        {
                            table.Cell().Element(CellStyle).Text(h.FuncionarioNome ?? "-");
                            table.Cell().Element(CellStyle).Text(h.ServicoDescricao ?? "-");
                            table.Cell().Element(CellStyle).Text(h.CarroId ?? "-");
                            table.Cell().Element(CellStyle).Text(h.ModeloNome ?? "-");
                            table.Cell().Element(CellStyle).Text(h.DataInicio?.ToString("dd/MM/yyyy") ?? "-");
                            table.Cell().Element(CellStyle).Text(h.DataPrevista?.ToString("dd/MM/yyyy") ?? "-");
                            table.Cell().Element(CellStyle).Text(h.Status ?? "-");
                            table.Cell().Element(CellStyle).Text(h.Acao ?? "-");
                            table.Cell().Element(CellStyle).Text(h.DataRegistro.ToString("dd/MM/yyyy HH:mm"));
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

        static IContainer CellStyle(IContainer container) {
            return container.PaddingVertical(5).PaddingHorizontal(5);
        }
    }
}
