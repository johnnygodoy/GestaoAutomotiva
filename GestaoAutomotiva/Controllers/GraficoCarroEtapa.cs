using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using GestaoAutomotiva.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace GestaoAutomotiva.Controllers
{
    public class GraficoCarroEtapaController : Controller
    {
        private readonly AppDbContext _context;

        public GraficoCarroEtapaController(AppDbContext context) {
            _context = context;
        }

        public IActionResult Index(string busca = null, DateTime? dataInicio = null, DateTime? dataFim = null, int? etapaId = null, string status = null) {
            var atividades = _context.Atividades
                .Include(a => a.Funcionario)
                .Include(a => a.Servico)
                .Include(a => a.Etapa)
                .Include(a => a.Carro).ThenInclude(c => c.Cliente)
                .ToList();

            // 🔍 Filtro de texto único
            if (!string.IsNullOrWhiteSpace(busca))
            {
                var termo = busca.ToLower();
                atividades = atividades.Where(a =>
                    (a.Funcionario?.Nome?.ToLower().Contains(termo) ?? false) ||
                    (a.Carro?.Modelo?.ToLower().Contains(termo) ?? false) ||
                    (a.Carro?.Cliente?.Nome?.ToLower().Contains(termo) ?? false)
                ).ToList();
            }

            // 📆 Filtros adicionais
            var atividadesFiltradas = atividades.Where(a =>
                (!dataInicio.HasValue || a.DataInicio >= dataInicio) &&
                (!dataFim.HasValue || a.DataPrevista <= dataFim) &&
                (!etapaId.HasValue || a.EtapaId == etapaId) &&
                (string.IsNullOrEmpty(status) || a.Status == status)
            ).ToList();

            // 🎨 Status visual
            foreach (var a in atividadesFiltradas)
            {
                a.Cor = a.Atrasado ? "#e74c3c" : "#2ecc71"; // vermelho se está atrasado
            }


            ViewBag.Etapas = _context.Etapas
                .Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.Nome })
                .ToList();

            return View(atividadesFiltradas);
        }


        [HttpPost]
        public IActionResult GerarPdf(IFormFile grafico) {
            using var stream = new MemoryStream();
            grafico.CopyTo(stream);
            var imagem = stream.ToArray();

            var relatorio = new RelatorioGraficoCarroEtapasPdf(imagem);
            var pdf = relatorio.GeneratePdf();

            return File(pdf, "application/pdf", "grafico_etapas.pdf");
        }
    }
}
