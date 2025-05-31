using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GestaoAutomotiva.Controllers
{
    public class EtapaController : Controller
    {
        private readonly AppDbContext _context;

        public EtapaController(AppDbContext context) {
            _context = context;
        }

        public IActionResult Index() {
            var etapas = _context.Etapas
                .OrderBy(e => e.Ordem)
                .ToList();

            return View(etapas);
        }

        public IActionResult Create() {
            ViewData["Etapas"] = _context.Etapas
                .OrderBy(e => e.Ordem)
                .ToList();

            return View(new Etapa());
        }

        [HttpPost]
        public IActionResult Create(Etapa etapa) {
            if (!ModelState.IsValid || !ValidarEtapa(etapa))
                return View(etapa);

            AjustarOrdemEtapasParaInsercao(etapa.Ordem);

            _context.Etapas.Add(etapa);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id) {
            var etapa = _context.Etapas.Find(id);
            if (etapa == null) return NotFound();

            return View(etapa);
        }

        [HttpPost]
        public IActionResult Edit(Etapa etapa) {
            if (!ModelState.IsValid || !ValidarEtapa(etapa))
                return View(etapa);

            var etapaExistente = _context.Etapas.Find(etapa.Id);
            if (etapaExistente == null) return NotFound();

            if (etapa.Ordem != etapaExistente.Ordem)
                ReorganizarOrdemEmEdicao(etapaExistente.Ordem, etapa.Ordem);

            etapaExistente.Nome = etapa.Nome.ToUpper();
            etapaExistente.Ordem = etapa.Ordem;

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id) {
            var etapa = _context.Etapas.Find(id);
            if (etapa == null) return NotFound();

            return View(etapa);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id) {
            var etapa = _context.Etapas.Find(id);
            if (etapa == null) return NotFound();

            _context.Etapas.Remove(etapa);

            var etapasAfetadas = _context.Etapas
                .Where(e => e.Ordem > etapa.Ordem)
                .ToList();

            foreach (var item in etapasAfetadas)
                item.Ordem -= 1;

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // =====================
        // MÉTODOS AUXILIARES
        // =====================

        private bool ValidarEtapa(Etapa etapa) {
            bool temErro = false;

            if (string.IsNullOrWhiteSpace(etapa.Nome))
            {
                ModelState.AddModelError("Nome", "O campo nome é obrigatório.");
                temErro = true;
            }

            if (etapa.Ordem <= 0)
            {
                ModelState.AddModelError("Ordem", "A ordem deve ser maior que zero.");
                temErro = true;
            }

            return !temErro;
        }

        private void AjustarOrdemEtapasParaInsercao(int novaOrdem) {
            var etapasAfetadas = _context.Etapas
                .Where(e => e.Ordem >= novaOrdem)
                .ToList();

            foreach (var item in etapasAfetadas)
                item.Ordem += 1;

            _context.SaveChanges();
        }

        private void ReorganizarOrdemEmEdicao(int ordemOriginal, int novaOrdem) {
            if (novaOrdem < ordemOriginal)
            {
                var etapas = _context.Etapas
                    .Where(e => e.Ordem >= novaOrdem && e.Ordem < ordemOriginal)
                    .ToList();

                foreach (var item in etapas)
                    item.Ordem += 1;
            }
            else if (novaOrdem > ordemOriginal)
            {
                var etapas = _context.Etapas
                    .Where(e => e.Ordem <= novaOrdem && e.Ordem > ordemOriginal)
                    .ToList();

                foreach (var item in etapas)
                    item.Ordem -= 1;
            }

            _context.SaveChanges();
        }
    }
}
