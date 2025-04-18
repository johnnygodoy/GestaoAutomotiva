using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using GestaoAutomotiva.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace GestaoAutomotiva.Controllers
{
    public class OrdemServicoController : Controller
    {
        private readonly AppDbContext _context;

        public OrdemServicoController(AppDbContext context) {
            _context = context;
        }

        // 🔁 Método auxiliar para carregar dropdowns
        private void CarregarViewBags(OrdemServico ordem = null) {
            ViewBag.Funcionarios = new SelectList(_context.Funcionarios, "Id", "Nome", ordem.FuncionarioId);
            ViewBag.Carros = new SelectList(_context.Carros, "Id", "Modelo", ordem.CarroId);
            ViewBag.Clientes = new SelectList(_context.Clientes, "Id", "Nome", ordem.ClienteId);

        }


        public IActionResult Index() {
            var ordens = _context.OrdemServicos
                .Include(o => o.Atividade)
                    .ThenInclude(a => a.Funcionario)
                .Include(o => o.Atividade)
                    .ThenInclude(a => a.Servico)
                .Include(o => o.Atividade)
                    .ThenInclude(a => a.Carro)
                        .ThenInclude(c => c.Cliente)
                .ToList();

            return View(ordens);
        }

        public IActionResult CriarOuEditar(int atividadeId) {
            var ordemExistente = _context.OrdemServicos.FirstOrDefault(o => o.AtividadeId == atividadeId);

            if (ordemExistente != null)
                return RedirectToAction("Edit", new { id = ordemExistente.Id });

            return RedirectToAction("Create", new { atividadeId });
        }


        public IActionResult Create(int? atividadeId) {
            OrdemServico ordem;

            if (atividadeId.HasValue)
            {
                var atividade = _context.Atividades
                    .Include(a => a.Funcionario)
                    .Include(a => a.Servico)
                    .Include(a => a.Etapa)
                    .Include(a => a.Carro).ThenInclude(c => c.Cliente)
                    .FirstOrDefault(a => a.Id == atividadeId.Value);

                if (atividade == null) return NotFound();

                ordem = new OrdemServico
                {
                    AtividadeId = atividade.Id,
                    Atividade = atividade,
                    DataAbertura = DateTime.Today,
                    EtapaAtual = atividade.Etapa?.Nome,
                    Prioridade = "Normal",
                    FuncionarioId = atividade.Funcionario?.Id,
                    CarroId = atividade.Carro?.Id,
                    ClienteId = atividade.Carro?.Cliente?.Id
                };

            }
            else
            {
                ordem = new OrdemServico
                {
                    DataAbertura = DateTime.Today,
                    EtapaAtual = "N/A",
                    Prioridade = "Normal"
                };
            }

    
            CarregarViewBags(ordem);
            return View(ordem);
        }


        [HttpPost]
        public IActionResult Create(OrdemServico ordem) {

            bool temErro = false;
            if (ordem.EtapaAtual == null)
            {
                ModelState.AddModelError("EtapaAtual", "O campo Etapa Atual é obrigatória.");
                temErro = true;
            }

            if (ordem.Prioridade == null)
            {
                ModelState.AddModelError("Prioridade", "O campo Prioridade é obrigatória.");
                temErro = true;
            }
        

            if (temErro)
            {
                CarregarViewBags(ordem); // apenas chama
                return View(ordem);
            }

            _context.OrdemServicos.Add(ordem);
            _context.SaveChanges();
            return RedirectToAction("Index");

        }

        public IActionResult Edit(int id) {
            var ordem = _context.OrdemServicos
                .Include(o => o.Atividade).ThenInclude(a => a.Funcionario)
                .Include(o => o.Atividade).ThenInclude(a => a.Carro).ThenInclude(c => c.Cliente)
                .Include(o => o.Atividade).ThenInclude(a => a.Servico)
                .FirstOrDefault(o => o.Id == id);

            if (ordem == null) return NotFound();

            ViewBag.Funcionarios = _context.Funcionarios
                .Select(f => new SelectListItem
                {
                    Value = f.Id.ToString(),
                    Text = f.Nome
                })
                .ToList();

            ViewBag.Carros = _context.Carros
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Modelo
                })
                .Distinct()
                .ToList();

            ViewBag.Clientes = _context.Clientes
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nome
                })
                .ToList();

            return View("Edit", ordem);
        }


        [HttpPost]
        public IActionResult Edit(OrdemServico ordem) {
            if (ModelState.IsValid)
            {
                _context.OrdemServicos.Update(ordem);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            CarregarViewBags(ordem);
            return View(ordem);
        }

        public IActionResult Delete(int id) {
            var ordem = _context.OrdemServicos
                .Include(o => o.Atividade).ThenInclude(a => a.Carro).ThenInclude(c => c.Cliente)
                .FirstOrDefault(o => o.Id == id);

            if (ordem == null) return NotFound();

            return View(ordem);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id) {
            var ordem = _context.OrdemServicos.Find(id);
            if (ordem == null) return NotFound();

            _context.OrdemServicos.Remove(ordem);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id) {
            var ordem = _context.OrdemServicos
                .Include(o => o.Atividade).ThenInclude(a => a.Funcionario)
                .Include(o => o.Atividade).ThenInclude(a => a.Carro).ThenInclude(c => c.Cliente)
                .Include(o => o.Atividade).ThenInclude(a => a.Servico)
                .FirstOrDefault(o => o.Id == id);

            if (ordem == null) return NotFound();
            return View(ordem);
        }

        public IActionResult Visualizar(int id) {
            return Details(id);
        }

        public IActionResult ExportarPdf(int id) {
            var ordem = _context.OrdemServicos
                .Include(o => o.Atividade).ThenInclude(a => a.Funcionario)
                .Include(o => o.Atividade).ThenInclude(a => a.Carro).ThenInclude(c => c.Cliente)
                .Include(o => o.Atividade).ThenInclude(a => a.Servico)
                .FirstOrDefault(o => o.Id == id);

            if (ordem == null)
                return NotFound();

            var doc = new RelatorioOrdemServicoPdf(ordem); // agora 1 única OS
            var pdf = doc.GeneratePdf();

            return File(pdf, "application/pdf", $"ordem_servico_{id}.pdf");
        }

    }
}
