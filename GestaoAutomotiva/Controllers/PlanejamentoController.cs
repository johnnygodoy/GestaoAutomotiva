using GestaoAutomotiva.Data;
using GestaoAutomotiva.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System.Net;

public class PlanejamentoController : Controller
{
    private readonly AppDbContext _context;

    public PlanejamentoController(AppDbContext context) {
        _context = context;
    }

    public IActionResult Index(
        int? funcionarioId = null,
        string tipoConflito = "Funcionario",
        DateTime? dataInicio = null,
        DateTime? dataFim = null,
        string status = null) {
        // 🔐 Carrega e já filtra atividades com DataInicio/DataPrevista nulas
        var atividades = _context.Atividades
       .Include(a => a.Funcionario)
       .Include(a => a.Carro).ThenInclude(c => c.Cliente)
       .Include(a => a.Carro).ThenInclude(c => c.Modelo)
       .Include(a => a.Servico)
       .Where(a => a.DataInicio.HasValue && a.DataPrevista.HasValue && a.Status != "Finalizado")
       .ToList();


        // 🧠 Filtros ANTES do processamento de conflitos
        if (funcionarioId.HasValue)
            atividades = atividades.Where(a => a.FuncionarioId == funcionarioId).ToList();

        if (dataInicio.HasValue)
            atividades = atividades.Where(a => a.DataInicio >= dataInicio.Value).ToList();

        if (dataFim.HasValue)
            atividades = atividades.Where(a => a.DataPrevista <= dataFim.Value).ToList();

        if (!string.IsNullOrWhiteSpace(status))
            atividades = atividades.Where(a => a.Status == status).ToList();

        // 🔍 Identifica conflitos
        var atividadesComConflito = atividades.Select(a =>
        {
            bool conflito = false;

            foreach (var other in atividades)
            {
                if (a.Id == other.Id) continue;

                bool sobrepoe = a.DataInicio <= other.DataPrevista &&
                                a.DataPrevista >= other.DataInicio;

                if (!sobrepoe) continue;

                switch (tipoConflito)
                {
                    case "Funcionario":
                        if (a.FuncionarioId == other.FuncionarioId) conflito = true;
                        break;
                    case "Carro":
                        if (a.CarroId == other.CarroId) conflito = true;
                        break;
                    case "Servico":
                        if (a.ServicoId == other.ServicoId) conflito = true;
                        break;
                    case "Status":
                        if (a.Status == other.Status) conflito = true;
                        break;
                    case "Qualquer":
                        conflito = true;
                        break;
                }

                if (conflito) break;
            }

            a.Conflito = conflito;
            return a;
        }).ToList();

        // 🎨 Cores
        string GetCorPorTipo(string tipo, bool conflito) {
            if (!conflito) return "#3498db";
            return tipo switch
            {
                "Funcionario" => "#e74c3c",
                "Carro" => "#f39c12",
                "Servico" => "#2980b9",
                "Status" => "#8e44ad",
                "Qualquer" => "#2ecc71",
                _ => "#7f8c8d"
            };
        }

        foreach (var a in atividadesComConflito)
            a.Cor = GetCorPorTipo(tipoConflito, a.Conflito);

        // 📊 Dados para gráficos
        ViewBag.TemConflitos = atividadesComConflito.Any(a => a.Conflito);

        ViewBag.FuncionariosConflito = atividadesComConflito
            .Where(a => a.Conflito)
            .Select(a => WebUtility.HtmlDecode(a.Funcionario.Nome))
            .Distinct()
            .ToList();

        ViewBag.ConflitosPorFuncionario = atividadesComConflito
            .Where(a => a.Conflito)
            .GroupBy(a => a.Funcionario.Nome)
            .Select(g => new { Funcionario = g.Key, Total = g.Count() })
            .ToList();

        ViewBag.TotalConflitos = atividadesComConflito.Count(a => a.Conflito);

        ViewBag.Funcionarios = _context.Funcionarios
            .Select(f => new SelectListItem { Value = f.Id.ToString(), Text = f.Nome })
            .ToList();

        return View(atividadesComConflito);
    }



    [HttpPost]
    public IActionResult GerarPdf(IFormFile grafico) {
        if (grafico == null || grafico.Length == 0)
            return BadRequest("Nenhum gráfico foi enviado.");

        using var stream = new MemoryStream();
        grafico.CopyTo(stream);
        var imagemBytes = stream.ToArray();

        var pdf = new RelatorioPlanejamentoPdf(imagemBytes);
        var arquivo = pdf.GeneratePdf();     

        return File(arquivo, "application/pdf", "planejamento.pdf");
    }



}