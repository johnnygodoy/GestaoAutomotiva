using GestaoAutomotiva.Data;
using GestaoAutomotiva.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace GestaoAutomotiva.Controllers
{
    public class HistoricoAtividadeController : Controller
    {
        private readonly AppDbContext _context;

        public HistoricoAtividadeController(AppDbContext context) {
            _context = context;
        }

        public IActionResult Index(string busca, string dataBusca = null, int page = 1) {
            int pageSize = 5;
            var historico = _context.AtividadeHistoricos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                var buscaUpper = busca.ToUpper();

                historico = historico.Where(h =>
                    h.FuncionarioNome.ToUpper().Contains(buscaUpper) ||
                    h.ServicoDescricao.ToUpper().Contains(buscaUpper) ||
                    h.ModeloNome.ToUpper().Contains(buscaUpper) ||
                    h.CarroId.ToUpper().Contains(buscaUpper) ||
                    h.Cliente.ToUpper().Contains(buscaUpper) ||
                    h.Status.ToUpper().Contains(buscaUpper));
            }

            if (!string.IsNullOrWhiteSpace(dataBusca) && DateTime.TryParse(dataBusca, out DateTime dataBuscaConvertida))
            {
                historico = historico.Where(h =>
                    h.DataInicio == dataBuscaConvertida.Date ||
                    h.DataPrevista == dataBuscaConvertida.Date);
            }

            var totalRegistros = historico.Count();
            var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)pageSize);

            var paginado = historico
                .OrderByDescending(h => h.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PaginaAtual = page;
            ViewBag.BuscaNome = busca;

            return View(paginado);
        }
        // public IActionResult ExportarPdf() {
        //     var historico = _context.AtividadeHistoricos
        //.OrderByDescending(h => h.Id)
        //.ToList();

        //     var documento = new RelatorioAtividadePdf(historico);
        //     var pdfBytes = documento.GeneratePdf(); // Isso retorna byte[]

        //     return File(pdfBytes, "application/pdf", "historico_atividades.pdf");

        // }

        public IActionResult ExportarPdf(string busca, string dataBusca = null, int page = 1) {
            int pageSize = 5;
            var historico = _context.AtividadeHistoricos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                var buscaUpper = busca.ToUpper();

                historico = historico.Where(h =>
                    h.FuncionarioNome.ToUpper().Contains(buscaUpper) ||
                    h.ServicoDescricao.ToUpper().Contains(buscaUpper) ||
                    h.ModeloNome.ToUpper().Contains(buscaUpper) ||
                    h.CarroId.ToUpper().Contains(buscaUpper) ||
                    h.Cliente.ToUpper().Contains(buscaUpper) ||
                    h.Status.ToUpper().Contains(buscaUpper));
            }

            if (!string.IsNullOrWhiteSpace(dataBusca) && DateTime.TryParse(dataBusca, out DateTime dataConvertida))
            {
                historico = historico.Where(h =>
                    h.DataInicio == dataConvertida.Date ||
                    h.DataPrevista == dataConvertida.Date);
            }

            var paginado = historico
                .OrderByDescending(h => h.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var documento = new RelatorioAtividadePdf(paginado);
            var pdfBytes = documento.GeneratePdf();

            return File(pdfBytes, "application/pdf", "historico_atividades.pdf");
        }

        public IActionResult ExportarExcel(string busca, string dataBusca = null, int page = 1) {
            int pageSize = 5;
            var historico = _context.AtividadeHistoricos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                var buscaUpper = busca.ToUpper();

                historico = historico.Where(h =>
                    h.FuncionarioNome.ToUpper().Contains(buscaUpper) ||
                    h.ServicoDescricao.ToUpper().Contains(buscaUpper) ||
                    h.ModeloNome.ToUpper().Contains(buscaUpper) ||
                    h.CarroId.ToUpper().Contains(buscaUpper) ||
                    h.Cliente.ToUpper().Contains(buscaUpper) ||
                    h.Status.ToUpper().Contains(buscaUpper));
            }

            if (!string.IsNullOrWhiteSpace(dataBusca) && DateTime.TryParse(dataBusca, out DateTime dataConvertida))
            {
                historico = historico.Where(h =>
                    h.DataInicio == dataConvertida.Date ||
                    h.DataPrevista == dataConvertida.Date);
            }

            var paginado = historico
                .OrderByDescending(h => h.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Histórico");

            // Cabeçalhos
            worksheet.Cell(1, 1).Value = "Funcionário";
            worksheet.Cell(1, 2).Value = "Serviço";
            worksheet.Cell(1, 3).Value = "Código Carro";
            worksheet.Cell(1, 4).Value = "Carro";
            worksheet.Cell(1, 5).Value = "Cliente";
            worksheet.Cell(1, 6).Value = "Início";
            worksheet.Cell(1, 7).Value = "Previsão";
            worksheet.Cell(1, 8).Value = "Status";
            worksheet.Cell(1, 9).Value = "Ação";
            worksheet.Cell(1, 10).Value = "Registro";

            for (int i = 0; i < paginado.Count; i++)
            {
                var h = paginado[i];

                worksheet.Cell(i + 2, 1).Value = h.FuncionarioNome ?? "-";
                worksheet.Cell(i + 2, 2).Value = h.ServicoDescricao ?? "-";
                worksheet.Cell(i + 2, 3).Value = h.CarroId ?? "-";
                worksheet.Cell(i + 2, 4).Value = h.ModeloNome ?? "-";
                worksheet.Cell(i + 2, 5).Value = h.Cliente ?? "-";
                worksheet.Cell(i + 2, 6).Value = h.DataInicio?.ToString("dd/MM/yyyy") ?? "-";
                worksheet.Cell(i + 2, 7).Value = h.DataPrevista?.ToString("dd/MM/yyyy") ?? "-";
                worksheet.Cell(i + 2, 8).Value = h.Status ?? "-";
                worksheet.Cell(i + 2, 9).Value = h.Acao ?? "-";
                worksheet.Cell(i + 2, 10).Value = h.DataRegistro.ToString("dd/MM/yyyy HH:mm");
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "historico_atividades.xlsx"
            );
        }


    }
}
