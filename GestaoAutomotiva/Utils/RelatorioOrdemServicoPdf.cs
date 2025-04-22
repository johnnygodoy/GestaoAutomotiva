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
                page.Margin(20);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(11));

                // ✅ Conteúdo principal
                page.Content().Border(1).Padding(15).Column(col =>
                {
                    // ✅ Banner no topo
                    col.Item().Image(logoPath);

                    // Número da OS e Data
                    col.Item().PaddingVertical(10).Row(row =>
                    {
                        row.RelativeItem().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn();
                                c.RelativeColumn();
                            });

                            table.Cell().Border(1).Padding(5).Text("Número da OS").Bold();
                            table.Cell().Border(1).Padding(5).Text(_ordem.Id.ToString());

                            table.Cell().Border(1).Padding(5).Text("Data de abertura").Bold();
                            table.Cell().Border(1).Padding(5).Text(_ordem.DataAbertura.ToString("dd/MM/yyyy"));
                        });
                    });

                    // Serviço a ser prestado
                    col.Item().PaddingBottom(10).AlignCenter().Text(txt =>
                    {
                        txt.Span("Serviço a ser prestado: ").Bold().FontSize(13);
                        txt.Span(_ordem.Atividade?.Servico?.Descricao ?? "").FontSize(13);
                    });

                    // 🧾 Tabela de dados da OS
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn();
                            c.RelativeColumn();
                        });

                        void Add(string label, string value) {
                            table.Cell().Border(1).Padding(5).Text(label).Bold();
                            table.Cell().Border(1).Padding(5).Text(value ?? "");
                        }

                        Add("Prioridade", _ordem.Prioridade);
                        Add("Colaborador", _ordem.Atividade?.Funcionario?.Nome);
                        Add("Modelo de Carro", _ordem.Atividade?.Carro?.Modelo);
                        Add("Cliente", _ordem.Atividade?.Carro?.Cliente?.Nome);
                        Add("Carro nº", _ordem.Atividade?.Carro?.IdCarro);
                        Add("Prazo", _ordem.Atividade?.DataPrevista?.ToString("dd/MM/yyyy"));
                    });

                    // ✅ Conferência (embaixo)
                    col.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn();
                            c.RelativeColumn();
                        });

                        table.Cell().ColumnSpan(2).Border(1).AlignCenter().Padding(5).Text("Conferência (com data)").Bold();

                        void Conf(string label, string assinatura) {
                            table.Cell().Border(1).Padding(5).Text(label);
                            table.Cell().Border(1).Padding(5).Text(assinatura ?? "");
                        }

                        Conf("Almoxarifado", _ordem.Almoxarifado);
                        Conf("Colaborador", _ordem.Atividade?.Funcionario?.Nome);
                        Conf("Inspetor", _ordem.Inspetor);
                    });

                    // Tarefas
                    col.Item().PaddingTop(15).Text("TAREFAS").Bold();
                    col.Item().Border(1).Height(60).Padding(5).Text(_ordem.Tarefas ?? "");

                    // Observações
                    col.Item().PaddingTop(10).Text("Observações:").Bold();
                    col.Item().Border(1).Height(100).Padding(5).Text(_ordem.Observacoes ?? "");

                    // Espaço entre tabelas e rodapé
                    col.Item().PaddingTop(30);

                    // Início e Término - Centralizado
                    col.Item().AlignCenter().Row(row =>
                    {
                        void Hora(string label) {
                            row.ConstantItem(80).Text(label).AlignLeft();
                            row.ConstantItem(120).BorderBottom(1).Height(12);
                            row.ConstantItem(40); // Espaço entre
                        }

                        Hora("INÍCIO:   HS");
                        Hora("TÉRMINO:   HS");
                    });

                    // Espaço
                    col.Item().PaddingTop(25);

                    // Nome e Assinatura
                    col.Item().Row(row =>
                    {
                        // Nome
                        row.RelativeItem().Column(col2 =>
                        {
                            col2.Item().BorderBottom(1).Height(15);
                            col2.Item().AlignCenter().Text("Nome do Responsável").FontSize(9);
                        });

                        row.ConstantItem(40); // Espaço entre

                        // Assinatura
                        row.RelativeItem().Column(col2 =>
                        {
                            col2.Item().BorderBottom(1).Height(15);
                            col2.Item().AlignCenter().Text("Assinatura").FontSize(9);
                        });
                    });
                });

                // ✅ Rodapé
                page.Footer().AlignCenter().Text(txt =>
                {
                    txt.Span("Documento gerado por GestãoAutomotiva - ");
                    txt.Span(DateTime.Now.ToString("dd/MM/yyyy")).SemiBold();
                });
            });
        }
    }
}
