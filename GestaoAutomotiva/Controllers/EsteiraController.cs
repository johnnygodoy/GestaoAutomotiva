using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
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
        public IActionResult AvancarEtapa(int id) {
            var atividade = _context.Atividades.Include(a => a.Etapa).FirstOrDefault(a => a.Id == id);
            if (atividade == null) return NotFound();

            var proxima = _context.Etapas
                .Where(e => e.Ordem > atividade.Etapa.Ordem)
                .OrderBy(e => e.Ordem)
                .FirstOrDefault();

            if (proxima != null)
            {
                atividade.EtapaId = proxima.Id;
                if (proxima.Nome == "Finalizado")
                    atividade.Status = "Finalizado";
                _context.SaveChanges();
            }

            return RedirectToAction("Esteira");
        }

        [HttpPost]
        public IActionResult VoltarEtapa(int id) {
            var atividade = _context.Atividades.Include(a => a.Etapa).FirstOrDefault(a => a.Id == id);
            if (atividade == null) return NotFound();

            var anterior = _context.Etapas
                .Where(e => e.Ordem < atividade.Etapa.Ordem)
                .OrderByDescending(e => e.Ordem)
                .FirstOrDefault();

            if (anterior != null)
            {
                atividade.EtapaId = anterior.Id;
                atividade.Status = "Em Andamento";
                _context.SaveChanges();
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
