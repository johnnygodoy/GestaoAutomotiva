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
            // 🔍 Query base com includes
            var query = _context.Atividades
                .Include(a => a.Funcionario)
                .Include(a => a.Servico)
                .Include(a => a.Etapa)
                .Include(a => a.Carro)
                    .ThenInclude(c => c.Cliente)
                .Include(a => a.Carro)
                    .ThenInclude(c => c.Modelo)
                .AsQueryable();

            // 🔎 Filtro por texto
            if (!string.IsNullOrWhiteSpace(busca))
            {
                var termo = busca.ToLower();
                query = query.Where(a =>
                    a.Funcionario.Nome.ToLower().Contains(termo) ||
                    a.Carro.Cliente.Nome.ToLower().Contains(termo) ||
                    a.Carro.Modelo.Nome.ToLower().Contains(termo)
                );
            }

            // 📆 Filtros adicionais
            if (dataInicio.HasValue)
                query = query.Where(a => a.DataInicio >= dataInicio);

            if (dataFim.HasValue)
                query = query.Where(a => a.DataPrevista <= dataFim);

            if (etapaId.HasValue)
                query = query.Where(a => a.EtapaId == etapaId);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(a => a.Status == status);

            // 🔄 Executa a query com todos os filtros aplicados no banco
            var atividadesFiltradas = query.ToList();

            // 🔁 Agrupamento para resumo por status
            var detalhesPorStatus = atividadesFiltradas
                .GroupBy(a => a.Status)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(a => new AtividadeResumo
                    {
                        Funcionario = a.Funcionario?.Nome,
                        Status = a.Status,
                        Servico = a.Servico?.Descricao,
                        Etapa = a.Etapa?.Nome
                    }).ToList()
                );

            ViewBag.DetalhesPorStatus = detalhesPorStatus;

            // 📊 Agrupamento por carro
            var carrosAgrupados = atividadesFiltradas
                .Where(a => a.DataInicio.HasValue && a.DataPrevista.HasValue)
                .GroupBy(a => a.Carro.IdCarro)
                .Select(g =>
                {
                    var atividadesResumo = g.Select(a => new AtividadeResumo
                    {
                        Funcionario = a.Funcionario?.Nome,
                        Status = a.Status,
                        Servico = a.Servico?.Descricao,
                        DiasAtraso = (a.Atrasado && a.DataPrevista.HasValue)
                            ? (int?)(DateTime.Now.Date - a.DataPrevista.Value.Date).Days
                            : null,
                        Etapa = a.Etapa?.Nome
                    }).ToList();

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
                        Tooltip = tooltip,
                        AtividadesResumo = atividadesResumo
                    };
                }).ToList();

            // 📋 Resumo por status
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

            // 📈 Resumo diário
            var dadosDiarios = atividadesFiltradas
      .Where(a => a.DataInicio.HasValue)
      .GroupBy(a => a.DataInicio.Value.Date)
      .Select(g => new DashboardDiarioViewModel
      {
          Data = g.Key,
          Concluidas = g.Count(a => a.Status?.ToUpper() == "FINALIZADO"),
          Andamento = g.Count(a => a.Status?.ToUpper() == "EM ANDAMENTO"),
          Canceladas = g.Count(a => a.Status?.ToUpper() == "CANCELADO"),
          Reprovadas = g.Count(a => a.Status?.ToUpper() == "REPROVADO"),
          Parados = g.Count(a => a.Status?.ToUpper() == "PARADO")
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

            // 🏷️ Nome do cliente/modelo para exibição
            var modeloFiltrado = carrosAgrupados.Select(c => c.Nome?.Split(" - ")[0]).Distinct().ToList();
            var clienteFiltrado = carrosAgrupados.Select(c => c.Nome?.Split(" - ")[1]).Distinct().ToList();

            ViewBag.ModeloFiltrado = modeloFiltrado.Count == 1 ? modeloFiltrado.First() : "Todos";
            ViewBag.ClienteFiltrado = clienteFiltrado.Count == 1 ? clienteFiltrado.First() : "Todos";

            return View(carrosAgrupados);
        }



        [HttpPost]
        public IActionResult GerarPdf(List<IFormFile> imagens) {
            if (imagens == null || imagens.Count == 0)
                return BadRequest("Nenhuma imagem recebida.");

            // Dicionário temporário para mapear pelo nome
            var mapa = new Dictionary<string, byte[]>();

            foreach (var arquivo in imagens)
            {
                using var stream = new MemoryStream();
                arquivo.CopyTo(stream);
                mapa[arquivo.FileName] = stream.ToArray();
            }

            // Ordem correta com títulos
            var ordem = new List<(string Nome, string Titulo)>
    {
        ("gantt.png", "Gráfico de Etapas"),
        ("status.png", "Visão Status"),
        ("pizza.png", "Visão Consolidada"),
        ("linha.png", "Visão Diária")
    };

            var imagensOrdenadas = new List<(string Titulo, byte[] Imagem)>();

            foreach (var (nome, titulo) in ordem)
            {
                if (mapa.ContainsKey(nome))
                    imagensOrdenadas.Add((titulo, mapa[nome]));
            }

            var relatorio = new RelatorioMultiplosGraficosPdf(imagensOrdenadas);
            var pdf = relatorio.GeneratePdf();

            return File(pdf, "application/pdf", "graficos_etapas.pdf");
        }





    }
}
