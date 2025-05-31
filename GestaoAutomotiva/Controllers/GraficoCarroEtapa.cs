using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using GestaoAutomotiva.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System;
using System.IO;
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
            // 🔍 Carrega atividades com todas as dependências necessárias
            var atividades = _context.Atividades
                .Include(a => a.Funcionario)
                .Include(a => a.Servico)
                .Include(a => a.Etapa)
                .Include(a => a.Carro)
                    .ThenInclude(c => c.Cliente)
                .Include(a => a.Carro)
                    .ThenInclude(c => c.Modelo) // ✅ Adicionado: garante acesso ao nome do modelo do carro
                .ToList();


            // 🔍 Aplica os filtros dinamicamente
            if (!string.IsNullOrWhiteSpace(busca))
            {
                var termo = busca.ToLower();
                atividades = atividades.Where(a =>
                    (a.Funcionario?.Nome?.ToLower().Contains(termo) ?? false) ||
                    (a.Carro?.Modelo?.Nome?.ToLower().Contains(termo) ?? false) ||
                    (a.Carro?.Cliente?.Nome?.ToLower().Contains(termo) ?? false)
                ).ToList();
            }

            var atividadesFiltradas = atividades.Where(a =>
                (!dataInicio.HasValue || a.DataInicio >= dataInicio) &&
                (!dataFim.HasValue || a.DataPrevista <= dataFim) &&
                (!etapaId.HasValue || a.EtapaId == etapaId) &&
                (string.IsNullOrWhiteSpace(status) || a.Status == status)
            ).ToList();



            // 📊 Agrupamento por carro
            var carrosAgrupados = atividadesFiltradas
                .Where(a => a.DataInicio.HasValue && a.DataPrevista.HasValue)
                .GroupBy(a => a.Carro.IdCarro)
                .Select(g =>
                {
                    var primeiro = g.First();

                    var clienteNome = TextoHelper.RemoverCaracteresEspeciais(primeiro.Carro.Cliente?.Nome ?? "Sem Cliente");
                    var modelo = TextoHelper.RemoverCaracteresEspeciais(primeiro.Carro.Modelo?.Nome ?? "Modelo Desconhecido");
                    var nomeCompleto = $"{modelo} - {clienteNome}";

                    var dataMin = g.Min(a => a.DataInicio.Value);
                    var dataMax = g.Max(a => a.DataPrevista.Value);

                    var total = g.Count();
                    var concluidas = g.Count(a => a.Status == "Finalizado");
                    var percentual = total > 0 ? (int)Math.Round((double)concluidas / total * 100) : 0;

                    var atrasado = g.Any(a => a.Atrasado);
                    var cor = percentual == 100 ? "#27ae60" : (atrasado ? "#c0392b" : "#2980b9");

                    var statusTexto = percentual == 100 ? "✅ Finalizado" :
                                      atrasado ? "🚨 Atrasado" :
                                      $"{percentual}% concluído";

                    var tooltip = $@"
                        <div style='padding:10px; font-size:13px;'>
                            <strong>Cliente:</strong> {clienteNome}<br />
                            <strong>Carro:</strong> {modelo}<br />
                            <strong>Status:</strong> {statusTexto}<br />
                            <strong>Início:</strong> {dataMin:dd/MM/yyyy}<br />
                            <strong>Previsão:</strong> {dataMax:dd/MM/yyyy}
                        </div>";

                    return new CarroResumoViewModel
                    {
                        Id = primeiro.Carro.Id,
                        Nome = nomeCompleto,
                        DataInicio = dataMin,
                        DataFim = dataMax,
                        Percentual = percentual,
                        Cor = cor,
                        Tooltip = tooltip
                    };
                })
                .ToList();

            // Contagem por status
            var totalAtividades = atividadesFiltradas.Count();
            var resumoPorStatus = atividadesFiltradas
                .GroupBy(a => a.Status)
                .Select(g => new DashboardResumoStatusViewModel
                {
                    Status = g.Key,
                    Total = g.Count(),
                    Percentual = totalAtividades > 0 ? (g.Count() * 100.0) / totalAtividades : 0
                }).ToList();

            ViewBag.ResumoPorStatus = resumoPorStatus;
            ViewBag.TotalAtividades = totalAtividades;

            // Agrupamento diário por status
            var dadosDiarios = atividadesFiltradas
                .Where(a => a.DataInicio.HasValue)
                .GroupBy(a => a.DataInicio.Value.Date)
                .Select(g => new DashboardDiarioViewModel
                {
                    Data = g.Key,
                    Concluidas = g.Count(a => a.Status == "Finalizado"),
                    Andamento = g.Count(a => a.Status == "Em Andamento"),
                    Canceladas = g.Count(a => a.Status == "Cancelado"),
                    Reprovadas = g.Count(a => a.Status == "Reprovado"),
                    Pendentes = g.Count(a => a.Status == "Pendente")
                })
                .OrderBy(d => d.Data)
                .ToList();

            ViewBag.DadosDiarios = dadosDiarios;


            // 📌 Dropdown de etapas
            ViewBag.Etapas = _context.Etapas
                .OrderBy(e => e.Ordem)
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Nome
                })
                .ToList();

            // 🏷️ Extrai modelo e cliente (se houver apenas um agrupamento)
            var modeloFiltrado = carrosAgrupados.Select(c => c.Nome?.Split(" - ")[0]).Distinct().ToList();
            var clienteFiltrado = carrosAgrupados.Select(c => c.Nome?.Split(" - ")[1]).Distinct().ToList();

            ViewBag.ModeloFiltrado = modeloFiltrado.Count == 1 ? modeloFiltrado.First() : "Todos";
            ViewBag.ClienteFiltrado = clienteFiltrado.Count == 1 ? clienteFiltrado.First() : "Todos";


            return View(carrosAgrupados);
        }

        [HttpPost]
        public IActionResult GerarPdf(IFormFile grafico) {
            if (grafico == null || grafico.Length == 0)
                return BadRequest("Imagem do gráfico não enviada.");

            using var stream = new MemoryStream();
            grafico.CopyTo(stream);

            var imagem = stream.ToArray();
            var relatorio = new RelatorioGraficoCarroEtapasPdf(imagem);
            var pdf = relatorio.GeneratePdf();

            return File(pdf, "application/pdf", "grafico_etapas.pdf");
        }
    }
}
