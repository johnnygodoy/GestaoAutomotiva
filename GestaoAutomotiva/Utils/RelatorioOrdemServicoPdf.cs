using GestaoAutomotiva.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GestaoAutomotiva.Utils
{
    public class RelatorioOrdemServicoPdf : IDocument
    {
        private readonly OrdemServico _ordem;

        public RelatorioOrdemServicoPdf(OrdemServico ordem) {
            _ordem = ordem;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container) {
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Furlan2.jpg");

            if (!File.Exists(logoPath))
                throw new FileNotFoundException("Imagem do logo não encontrada em: " + logoPath);

            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Column(header =>
                {
                    header.Item().AlignCenter().Image(logoPath, ImageScaling.FitWidth);

                    header.Item().PaddingVertical(5).Row(row =>
                    {
                        row.RelativeItem().Text($"Número da OS: {_ordem.Id}").Bold();
                        row.RelativeItem().AlignRight().Text($"Data de abertura: {_ordem.DataAbertura:dd/MM/yyyy}").Bold();
                    });

                    header.Item().PaddingBottom(5).Text(txt =>
                    {
                        header.Item().PaddingTop(5).AlignCenter().Text("SERVIÇO A SER PRESTADO").Bold().FontSize(12);
                        header.Item().PaddingBottom(5).AlignCenter().Text(_ordem.Atividade?.Servico?.Descricao ?? "Não informado").FontSize(11);

                    });
                });



                page.Content().Column(content =>
                {
                    // 🧾 Informações principais
                    content.Item().Table(table =>
                    {
                        table.ColumnsDefinition(c => c.RelativeColumn());

                        void AddLinha(string label, string valor) {
                            table.Cell().Border(1).Padding(3).AlignLeft().Text(text =>
                            {
                                text.Span(label + ": ").Bold();
                                text.Span(valor ?? "");
                            });
                        }
                        AddLinha("Código do Carro", _ordem.Atividade?.Carro?.IdCarro);
                        AddLinha("Modelo de Carro", _ordem.Atividade?.Carro?.Modelo?.Nome ?? "Modelo não informado");
                        AddLinha("Cliente", _ordem.Atividade?.Carro?.Cliente?.Nome);
                        AddLinha("Prioridade", _ordem.Prioridade);
                        AddLinha("Colaborador", _ordem.Atividade?.Funcionario?.Nome);                
                        AddLinha("Prazo", _ordem.Atividade?.DataPrevista?.ToString("dd/MM/yyyy"));
                    });

                    // ✅ Conferência (com mesmo estilo dos campos superiores)
                    content.Item().PaddingTop(10).Text("Conferência (com data)").Bold().AlignCenter();

                    content.Item().Table(table =>
                    {
                        table.ColumnsDefinition(c => c.RelativeColumn());

                        void AddLinha(string label, string valor) {
                            table.Cell().Border(1).Padding(3).AlignLeft().Text(text =>
                            {
                                text.Span(label + ": ").Bold();
                                text.Span(valor ?? "");
                            });
                        }

                        AddLinha("Almoxarifado", _ordem.Almoxarifado);
                        AddLinha("Colaborador", _ordem.Atividade?.Funcionario?.Nome);
                        AddLinha("Inspetor", _ordem.Inspetor);
                    });


                    // TAREFAS
                    content.Item().PaddingTop(12).Text("TAREFAS").Bold();
                    content.Item().Border(1).Height(120).Padding(5).Text(_ordem.Tarefas ?? "");

                    // OBSERVAÇÕES
                    content.Item().PaddingTop(10).Text("Observações:").Bold();
                    content.Item().Border(1).Height(120).Padding(5).Text(_ordem.Observacoes ?? "");
                });

                // ✅ Rodapé fixo
                page.Footer().Column(footer =>
                {
                    // INÍCIO / TÉRMINO
                    footer.Item().PaddingTop(10).AlignCenter().Row(row =>
                    {
                        void Hora(string label) {
                            row.ConstantItem(80).Text(label).AlignLeft();
                            row.ConstantItem(120).BorderBottom(1).Height(12);
                            row.ConstantItem(40);
                        }

                        Hora("INÍCIO: HS");
                        Hora("TÉRMINO: HS");
                    });

                    // Nome e Assinatura
                    footer.Item().PaddingTop(10).Row(row =>
                    {
                        row.RelativeItem().Column(col2 =>
                        {
                            col2.Item().BorderBottom(1).Height(15);
                            col2.Item().AlignCenter().Text("Nome do Responsável").FontSize(9);
                        });

                        row.ConstantItem(40);

                        row.RelativeItem().Column(col2 =>
                        {
                            col2.Item().BorderBottom(1).Height(15);
                            col2.Item().AlignCenter().Text("Assinatura").FontSize(9);
                        });
                    });

                    // Linha final
                    footer.Item().PaddingTop(10).AlignCenter().Text(txt =>
                    {
                        txt.Span("Documento gerado por GestãoAutomotiva - ");
                        txt.Span(DateTime.Now.ToString("dd/MM/yyyy")).SemiBold();
                    });
                });
            });
        }


    }
}
