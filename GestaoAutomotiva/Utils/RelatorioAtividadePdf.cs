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
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Element(e =>
                {
                    e.Container().PaddingBottom(15).AlignCenter().Text("Histórico de Atividades")
                     .Bold()
                     .FontSize(18);
                });


                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(90); // Funcionário
                        columns.ConstantColumn(90); // Serviço
                        columns.RelativeColumn();   // Carro
                        columns.ConstantColumn(70); // Placa
                        columns.ConstantColumn(70); // Início
                        columns.ConstantColumn(70); // Previsão
                        columns.ConstantColumn(70); // Status
                    });

                    // Cabeçalho
                    table.Header(header =>
                    {
                        header.Cell().Text("Funcionário").Bold();
                        header.Cell().Text("Serviço").Bold();
                        header.Cell().Text("Código do Carro").Bold();
                        header.Cell().Text("Placa").Bold();
                        header.Cell().Text("Início").Bold();
                        header.Cell().Text("Previsão").Bold();
                        header.Cell().Text("Status").Bold();
                    });

                    // Linhas
                    foreach (var a in _atividades)
                    {
                        table.Cell().Text(a.Funcionario?.Nome ?? "-");
                        table.Cell().Text(a.Servico?.Descricao ?? "-");
                        table.Cell().Text(a.Carro.IdCarro);
                        table.Cell().Text(a.Carro.Modelo);
                        table.Cell().Text(a.DataInicio?.ToString("dd/MM/yyyy"));
                        table.Cell().Text(a.DataPrevista?.ToString("dd/MM/yyyy"));
                        table.Cell().Text(a.Status);
                    }
                });

                page.Footer().AlignCenter().Text(txt =>
                {
                    txt.Span("Documento gerado por GestãoAutomotiva ");
                    txt.Span(DateTime.Now.ToString("dd/MM/yyyy")).SemiBold();
                });
            });
        }
    }
}
