using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;
using GestaoAutomotiva.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            var carro = _ordem.Atividade?.Carro;
            var cliente = carro?.Cliente;
            var modelo = carro?.Modelo;
            var acessorios = carro?.Acessorios;

            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Furlan2.jpg");
            var dataGeracao = DateTime.Now.ToString("yy");


            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontSize(11));

                // HEADER
                page.Header().Column(header =>
                {
                    // ✅ Logo centralizado
                    if (File.Exists(logoPath))
                        header.Item().AlignCenter().Image(logoPath, ImageScaling.FitWidth);

                    // ✅ Título e número
                    header.Item().AlignCenter().Text("ORDEM DE SERVIÇO").FontSize(20).Bold();
                    header.Item().AlignRight().Text($"Nº {_ordem.Id:00}/{dataGeracao}").FontSize(12).Bold();


                    // ✅ Informações do carro
                    header.Item().PaddingTop(10).Row(info =>
                    {
                        info.RelativeItem().Column(col =>
                        {
                            col.Item().Text($"CARRO Nº: {carro?.IdCarro ?? "___"}").Bold();
                            col.Item().Text($"MODELO: {modelo?.Nome ?? "___"}").Bold();
                            col.Item().Text($"CLIENTE: {cliente?.Nome ?? "___"}").Bold();
                        });

                        info.ConstantItem(180).Border(1.2f).Padding(5).Column(col =>
                        {
                            col.Item().Text($"MOTOR:  {acessorios?.Motor?.Nome ?? "___"}").Bold();
                            col.Item().Text($"CAMBIO:  {acessorios?.Cambio?.Descricao ?? "___"}").Bold();
                            col.Item().Text($"CARROCERIA:  {acessorios?.Carroceria?.Descricao ?? "___"}").Bold();
                            col.Item().Text($"CAPOTA:  {acessorios?.Capota?.Descricao ?? "___"}").Bold();
                            col.Item().Text($"SUSPENSÃO:  {acessorios?.Suspensao?.Descricao ?? "___"}").Bold();
                            col.Item().Text($"RODAS:  {acessorios?.Roda?.Descricao ?? "___"}").Bold();
                            col.Item().Text($"PNEUS:  {acessorios?.Pneu?.Descricao ?? "___"}").Bold();
                            col.Item().Text($"SANTO ANTÔNIO:  {acessorios?.SantoAntonio?.Descricao ?? "___"}").Bold();
                            col.Item().Text($"ESCAPAMENTO:  {acessorios?.Escapamento?.Descricao ?? "___"}").Bold();
                            col.Item().Text($"PAINEL:  {acessorios?.Painel?.Descricao ?? "___"}").Bold();
                        });
                    });
                });

                // CONTEÚDO PRINCIPAL
                page.Content().Column(content =>
                {
                    // Etapa
                    var etapa = _ordem.Atividade?.Etapa?.Nome ?? "____________________________";
                    content.Item().PaddingTop(10).AlignLeft().Text($"Etapa: {etapa}").FontSize(16).Bold();

                    // Bloco de conferência
                    content.Item().PaddingTop(10).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("CONFERÊNCIA INICIAL ____/____/____").Bold().FontSize(12);
                            col.Item().Text("ALMOXARIFADO:");
                            col.Item().Text("EDILSON:");
                            col.Item().Text("SUPERVISOR:");
                            col.Item().Text("COLABORADOR:");
                        });

                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Text("CONFERÊNCIA FINAL ____/____/____").Bold().FontSize(12);
                            col.Item().Text("SUPERVISOR:");
                            col.Item().Text("EDILSON:");
                            col.Item().Text("QUALIDADE:");
                            col.Item().Text("MICHELlE:");
                        });
                    });

                    // TAREFAS
                    var tarefas = (_ordem.Tarefas ?? "").Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                    content.Item().PaddingTop(10).Border(1.5f).Padding(5).MaxHeight(250).Column(col =>

                    {
                        col.Item().Text("TAREFAS:").Bold().FontSize(12);
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(t =>
                            {
                                for (int i = 0; i < Math.Min(5, tarefas.Length); i++)
                                    t.Item().Text(txt =>
                                    {
                                        txt.Span("☐").FontSize(20);
                                        txt.Span(" " + tarefas[i]);
                                    });
                            });

                            row.RelativeItem().Column(t =>
                            {
                                for (int i = 5; i < tarefas.Length; i++)
                                    t.Item().Text(txt =>
                                    {
                                        txt.Span("☐").FontSize(20);
                                        txt.Span(" " + tarefas[i]);
                                    });
                            });
                        });
                    });

                    // OBSERVAÇÕES
                    var observacoes = (_ordem.Observacoes ?? "").Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                    content.Item().PaddingTop(10).Border(1.5f).Padding(5).Column(col =>
                    {
                        col.Item().Text("OBSERVAÇÕES:").Bold().FontSize(12);

                        if (observacoes.Length > 0)
                        {
                            foreach (var linha in observacoes.Take(8))
                            {
                                col.Item().Text("• " + linha).FontSize(11);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                col.Item().Element(e => e
                                    .PaddingVertical(6)
                                    .ExtendHorizontal()
                                    .LineHorizontal(1)
                                    .LineColor(Colors.Grey.Darken2));
                            }
                        }
                    });
                });

                // ✅ FOOTER DEVE FICAR AQUI FORA do page.Content()
                page.Footer().PaddingTop(2).Column(footer =>
                {
                    footer.Item().PaddingBottom(30).Row(row =>
                    {
                        row.RelativeItem().Text("ITENS SOLICITADOS PARA COMPRA DATA: ____/____/____").FontSize(10);
                        row.RelativeItem().Text("RECEBIMENTO DOS ITENS DATA: ____/____/____").FontSize(10);
                    });

                    footer.Item().Row(row =>
                    {
                        row.RelativeItem().Text("INÍCIO: ____/____/____ - HS ____").FontSize(10);
                        row.RelativeItem().Text("TÉRMINO: ____/____/____ - HS ____").FontSize(10);
                        row.RelativeItem().Text("COLABORADOR: ").FontSize(12);
                    });

                    footer.Item().PaddingTop(20).PaddingBottom(30).AlignCenter().Text(txt =>
                    {
                        txt.Span("Gestão Automotiva - ").SemiBold();
                        txt.Span("Ordem de Serviço gerada automaticamente.");
                        txt.DefaultTextStyle(x => x.FontSize(8));
                    });
                });
            });

        }
    }
}
