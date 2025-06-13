using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using GestaoAutomotiva.Models.ViewModels;
using GestaoAutomotiva.Utils;
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

        public IActionResult Index(int page = 1) {
            
            int pageSize = 5;
            var etapas = _context.Etapas
                .OrderBy(e => e.Ordem)
                .ToList();

            var totalRegistros = etapas.Count();
            var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)pageSize);

            var etapasPaginadas = etapas
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new EtapaViewModel
            {
                ListaEtapas = etapasPaginadas,
                PaginaAtual = page,
                TotalPaginas = totalPaginas,
                Etapa = new Etapa()
            };

            return View(viewModel);
        }



        public IActionResult Create() {
            var etapas = _context.Etapas
                .OrderBy(e => e.Ordem)
                .ToList();

            var viewModel = new EtapaViewModel
            {
                Etapa = new Etapa(),
                ListaEtapas = etapas,
                PaginaAtual = 1,
                TotalPaginas = (int)Math.Ceiling(etapas.Count / 10.0)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EtapaViewModel viewModel) {
            if (!ValidarEtapa(viewModel.Etapa))
            {
                var etapas = _context.Etapas.OrderBy(e => e.Ordem).ToList();

                viewModel.ListaEtapas = etapas;
                viewModel.PaginaAtual = 1;
                viewModel.TotalPaginas = (int)Math.Ceiling(etapas.Count / 10.0);

                return View("Index", viewModel);
            }

            // 🔧 Reorganiza as etapas ANTES de adicionar a nova
            AjustarOrdemEtapasParaInsercao(viewModel.Etapa.Ordem);

            // 🔐 Força a ordem final a ser exatamente a desejada
            var novaOrdem = viewModel.Etapa.Ordem;
            viewModel.Etapa.Nome = viewModel.Etapa.Nome.ToUpper();

            _context.Etapas.Add(viewModel.Etapa);

            bool salvo = await RetryHelper.TentarSalvarAsync(async () =>
            {
                await _context.SaveChangesAsync();
            });

            if (!salvo)
            {
                TempData["Erro"] = "Erro ao salvar a nova etapa. Tente novamente.";
                return RedirectToAction("Index");
            }

            // 🔄 Reorganiza novamente após o salvamento
            ReorganizarTodasAsOrdens();

            return RedirectToAction("Index");
        }



        public IActionResult Edit(int id) {
            var etapa = _context.Etapas.Find(id);
            if (etapa == null) return NotFound();

            return View(etapa);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Etapa etapa) {
            if (!ValidarEtapa(etapa))
                return View(etapa);

            var etapaExistente = await _context.Etapas.FindAsync(etapa.Id);
            if (etapaExistente == null) return NotFound();

            if (etapa.Ordem != etapaExistente.Ordem)
                ReorganizarOrdemEmEdicao(etapaExistente.Ordem, etapa.Ordem);

            etapaExistente.Nome = etapa.Nome.ToUpper();
            etapaExistente.Ordem = etapa.Ordem;

            var salvo = await RetryHelper.TentarSalvarAsync(async () =>
            {
                await _context.SaveChangesAsync();
            });

            if (!salvo)
            {
                TempData["Erro"] = "Erro ao salvar a etapa editada. Tente novamente.";
                return RedirectToAction("Index");
            }

            ReorganizarTodasAsOrdens();
            return RedirectToAction("Index");
        }


        public IActionResult Delete(int id) {
            var etapa = _context.Etapas.Find(id);
            if (etapa == null) return NotFound();

            return View(etapa);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var etapa = await _context.Etapas.FindAsync(id);
            if (etapa == null) return NotFound();

            _context.Etapas.Remove(etapa);

            var etapasAfetadas = await _context.Etapas
                .Where(e => e.Ordem > etapa.Ordem)
                .ToListAsync();

            foreach (var item in etapasAfetadas)
                item.Ordem -= 1;

            var salvo = await RetryHelper.TentarSalvarAsync(async () =>
            {
                await _context.SaveChangesAsync();
            });

            if (!salvo)
            {
                TempData["Erro"] = "Erro ao excluir a etapa. Tente novamente.";
                return RedirectToAction("Index");
            }

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


        private async Task AjustarOrdemEtapasParaInsercao(int novaOrdem) {
            var etapas = await _context.Etapas
                .Where(e => e.Ordem >= novaOrdem)
                .OrderByDescending(e => e.Ordem)
                .ToListAsync();

            foreach (var etapa in etapas)
                etapa.Ordem += 1;

            await RetryHelper.TentarSalvarAsync(async () =>
            {
                await _context.SaveChangesAsync();
            });
        }



        private async Task ReorganizarTodasAsOrdens() {
            var etapasOrdenadas = await _context.Etapas
       .OrderBy(e => e.Ordem)
       .ToListAsync();

            for (int i = 0; i < etapasOrdenadas.Count; i++)
            {
                etapasOrdenadas[i].Ordem = i + 1;
            }

            await RetryHelper.TentarSalvarAsync(async () =>
            {
                await _context.SaveChangesAsync();
            });
        }

        private async Task ReorganizarOrdemEmEdicao(int ordemOriginal, int novaOrdem) {
            if (novaOrdem == ordemOriginal)
                return;

            if (novaOrdem < ordemOriginal)
            {
                // Ex: mover da ordem 4 para 2 → itens de 2 a 3 devem ir para 3 a 4
                var etapas = await _context.Etapas
                    .Where(e => e.Ordem >= novaOrdem && e.Ordem < ordemOriginal)
                    .OrderBy(e => e.Ordem)
                    .ToListAsync();

                foreach (var etapa in etapas)
                    etapa.Ordem += 1;
            }
            else
            {
                // Ex: mover da ordem 2 para 4 → itens de 3 a 4 devem ir para 2 a 3
                var etapas = await _context.Etapas
                    .Where(e => e.Ordem <= novaOrdem && e.Ordem > ordemOriginal)
                    .OrderBy(e => e.Ordem)
                    .ToListAsync();

                foreach (var etapa in etapas)
                    etapa.Ordem -= 1;
            }

            await RetryHelper.TentarSalvarAsync(async () =>
            {
                await _context.SaveChangesAsync();
            });
        }


    }
}
