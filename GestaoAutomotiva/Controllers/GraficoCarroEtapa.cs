using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using GestaoAutomotiva.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System;
using System.Linq;

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

            // 🔍 Filtro
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

            // 📊 Agrupar por carro
            var carrosAgrupados = atividadesFiltradas
                .Where(a => a.DataInicio.HasValue && a.DataPrevista.HasValue)
                .GroupBy(a => a.Carro.IdCarro)
                .Select(g =>
                {
                    var primeiro = g.First();
                    var clienteNome = TextoHelper.RemoverCaracteresEspeciais(primeiro.Carro.Cliente?.Nome);
                    var modelo = TextoHelper.RemoverCaracteresEspeciais(primeiro.Carro.Modelo);
                    var nomeCompleto = $"{modelo} - {clienteNome}";

                    var dataInicio = g.Min(a => a.DataInicio.Value);
                    var dataFim = g.Max(a => a.DataPrevista.Value);

                    var totalAtividades = g.Count();
                    var concluidas = g.Count(a => a.Status == "Finalizado");
                    var percentual = (int)Math.Round((double)concluidas / totalAtividades * 100);

                    var atrasado = g.Any(a => a.Atrasado);
                    var cor = percentual == 100 ? "#27ae60" : (atrasado ? "#c0392b" : "#2980b9");

                    var tooltip = $"<div style='padding:10px; font-size:13px;'>"
                                + $"<strong>Cliente:</strong> {clienteNome}<br />"
                                + $"<strong>Carro:</strong> {modelo}<br />"
                                + $"<strong>Status:</strong> {(percentual == 100 ? "✅ Finalizado" : atrasado ? "🚨 Atrasado" : $"{percentual}% concluído")}<br />"
                                + $"<strong>Início:</strong> {dataInicio:dd/MM/yyyy}<br />"
                                + $"<strong>Previsão:</strong> {dataFim:dd/MM/yyyy}</div>";

                    return new CarroResumoViewModel
                    {
                        Id = primeiro.Carro.Id,
                        Nome = nomeCompleto,
                        DataInicio = dataInicio,
                        DataFim = dataFim,
                        Percentual = percentual,
                        Cor = cor,
                        Tooltip = tooltip
                    };
                }).ToList();

            ViewBag.Etapas = _context.Etapas
                .Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.Nome })
                .ToList();

            return View(carrosAgrupados);
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
