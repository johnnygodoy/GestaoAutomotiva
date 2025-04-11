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
            var etapas = _context.Etapas.OrderBy(e => e.Ordem).ToList();
         
            return View(etapas);
        }

        public IActionResult Create() {
            var etapas = _context.Etapas.ToList();
            
            // Obtém todas as etapas
            ViewData["Etapas"] = etapas; // Armazena a lista de etapas em ViewData

            var etapa = new Etapa(); // Cria uma nova instância de Etapa
            return View(etapa); // Passa a nova etapa para a view
        }



        [HttpPost]
        public IActionResult Create(Etapa etapa) {
            bool temErro = false;

            if (string.IsNullOrEmpty(etapa.Nome))
            {
                ModelState.AddModelError("Nome", "O campo nome é obrigatório.");
                temErro = true;
            }
            else
            {
                etapa.Nome = etapa.Nome.ToUpper();
            }

            if (etapa.Ordem == null)
            {
                ModelState.AddModelError("Ordem", "O campo Ordem é obrigatório.");
                temErro = true;
            }
            else
            {
                var etapaExistente = _context.Etapas.FirstOrDefault(e => e.Ordem == etapa.Ordem);

                if (etapaExistente != null)
                {
                    // Aqui você pode alterar a ordem de todas as etapas após o valor atual, por exemplo:
                    var etapasAfetadas = _context.Etapas.Where(e => e.Ordem >= etapa.Ordem).ToList();
                    foreach (var item in etapasAfetadas)
                    {
                        item.Ordem += 1; // Incrementa a ordem das etapas seguintes
                    }
                    _context.SaveChanges();
                }
            }

            if (temErro)
            {
                return View(etapa);
            }

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
            bool temErro = false;

            if (string.IsNullOrEmpty(etapa.Nome))
            {
                ModelState.AddModelError("Nome", "O campo nome é obrigatório.");
                temErro = true;
            }

            else
            {
                etapa.Nome = etapa.Nome.ToUpper();
            }

            if (etapa.Ordem == null)
            {
                ModelState.AddModelError("Ordem", "O campo Ordem é obrigatório.");
                temErro = true;
            }

            if (temErro)
            {
                return View(etapa);
            }
            else
            {
                // Se a ordem foi alterada
                var etapaExistente = _context.Etapas.FirstOrDefault(e => e.Id == etapa.Id);
                if (etapaExistente != null)
                {
                    // Verifique se a ordem foi alterada
                    if (etapa.Ordem != etapaExistente.Ordem)
                    {
                        // Se a nova ordem for menor, devemos deslocar as etapas para cima
                        if (etapa.Ordem < etapaExistente.Ordem)
                        {
                            var etapasAfetadas = _context.Etapas.Where(e => e.Ordem >= etapa.Ordem && e.Ordem < etapaExistente.Ordem).ToList();
                            foreach (var item in etapasAfetadas)
                            {
                                item.Ordem += 1; // Incrementa a ordem das etapas subsequentes
                            }
                        }
                        // Se a nova ordem for maior, devemos deslocar as etapas para baixo
                        else if (etapa.Ordem > etapaExistente.Ordem)
                        {
                            var etapasAfetadas = _context.Etapas.Where(e => e.Ordem <= etapa.Ordem && e.Ordem > etapaExistente.Ordem).ToList();
                            foreach (var item in etapasAfetadas)
                            {
                                item.Ordem -= 1; // Decrementa a ordem das etapas subsequentes
                            }
                        }
                    }

                    etapaExistente.Nome = etapa.Nome; // Atualiza o nome
                    etapaExistente.Ordem = etapa.Ordem; // Atualiza a ordem
                    _context.SaveChanges();
                }
            }

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

            // Remove a etapa
            _context.Etapas.Remove(etapa);

            // Corrige a ordem das etapas subsequentes
            var etapasAfetadas = _context.Etapas.Where(e => e.Ordem > etapa.Ordem).ToList();
            foreach (var item in etapasAfetadas)
            {
                item.Ordem -= 1; // Decrementa a ordem das etapas subsequentes
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
