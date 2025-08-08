using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using GestaoAutomotiva.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoAutomotiva.Controllers
{
    public class EsteiraController : Controller
    {
        private readonly AppDbContext _context;

        public EsteiraController(AppDbContext context) {
            _context = context;
        }

        public IActionResult Esteira() {
            var etapas = _context.Etapas.OrderBy(e => e.Ordem).ToList();
            var atividades = _context.Atividades
    .Include(a => a.Funcionario)
    .Include(a => a.Servico)
    .Include(a => a.Etapa)
    .Include(a => a.Carro)
        .ThenInclude(c => c.Cliente)
    .Include(a => a.Carro)
        .ThenInclude(c => c.Modelo)

                .ToList();

            var viewModel = new EsteiraViewModel
            {
                Etapas = etapas,
                Atividades = atividades
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> AvancarEtapa(int id) {
            var atividade = _context.Atividades.Include(a => a.Etapa).FirstOrDefault(a => a.Id == id);
            if (atividade == null)
                return NotFound();

            var proxima = _context.Etapas
                .Where(e => e.Ordem > atividade.Etapa.Ordem)
                .OrderBy(e => e.Ordem)
                .FirstOrDefault();

            if (proxima != null)
            {
                atividade.EtapaId = proxima.Id;

                if (proxima.Nome.ToUpper() == "FINALIZADO")
                    atividade.Status = "Finalizado";

                bool salvo = await RetryHelper.TentarSalvarAsync(async () =>
                {
                    await _context.SaveChangesAsync();
                });

                if (!salvo)
                {
                    TempData["Erro"] = "Erro ao avançar etapa. Tente novamente.";
                }
            }

            return RedirectToAction("Esteira");
        }


        [HttpPost]
        public async Task<IActionResult> VoltarEtapa(int id) {
            var atividade = _context.Atividades.Include(a => a.Etapa).FirstOrDefault(a => a.Id == id);
            if (atividade == null)
                return NotFound();

            var anterior = _context.Etapas
                .Where(e => e.Ordem < atividade.Etapa.Ordem)
                .OrderByDescending(e => e.Ordem)
                .FirstOrDefault();

            if (anterior != null)
            {
                atividade.EtapaId = anterior.Id;
                atividade.Status = "Em Andamento";

                bool salvo = await RetryHelper.TentarSalvarAsync(async () =>
                {
                    await _context.SaveChangesAsync();
                });

                if (!salvo)
                {
                    TempData["Erro"] = "Erro ao voltar etapa. Tente novamente.";
                }
            }

            return RedirectToAction("Esteira");
        }


        // Método para redirecionar para a tela de cadastro de etapas
        public IActionResult CadastroEtapas() {
            var etapas = _context.Etapas.OrderBy(e => e.Ordem).ToList();
            return View(etapas);
        }
    }
}
