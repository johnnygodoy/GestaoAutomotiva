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

        private void CarregarViewBags(OrdemServico ordem = null) {
            ViewBag.Funcionarios = new SelectList(_context.Funcionarios, "Id", "Nome", ordem?.FuncionarioId);
            ViewBag.Carros = new SelectList(_context.Carros, "Id", "Modelo", ordem?.CarroId);
            ViewBag.Clientes = new SelectList(_context.Clientes, "Id", "Nome", ordem?.ClienteId);
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
                .Include(o => o.Atividade)
                    .ThenInclude(a => a.Etapa)
                .ToList();

            ViewBag.Funcionarios = _context.Funcionarios
                .Select(f => new SelectListItem { Value = f.Id.ToString(), Text = f.Nome })
                .ToList();

            ViewBag.Clientes = _context.Clientes
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nome })
                .ToList();

            return View(ordens);
        }


        public IActionResult CriarOuEditar(int atividadeId) {
            var existente = _context.OrdemServicos.FirstOrDefault(o => o.AtividadeId == atividadeId);
            if (existente != null)
                return RedirectToAction("Edit", new { id = existente.Id });

            return RedirectToAction("Create", new { atividadeId });
        }

        public IActionResult Create(int? atividadeId) {


            OrdemServico ordem = new OrdemServico
            {
                DataAbertura = DateTime.Today,
                EtapaAtual = "N/A",
                Prioridade = "Normal"
            };

            if (atividadeId.HasValue)
            {
                var atividade = _context.Atividades
                    .Include(a => a.Funcionario)
                    .Include(a => a.Servico)
                    .Include(a => a.Carro).ThenInclude(c => c.Cliente)
                    .Include(a => a.Etapa)
                    .FirstOrDefault(a => a.Id == atividadeId.Value);

                if (atividade != null)
                {
                    ordem.AtividadeId = atividade.Id;
                    ordem.Atividade = atividade;
                    ordem.EtapaAtual = atividade.Etapa?.Nome ?? "N/A";
                    ordem.FuncionarioId = atividade.Funcionario?.Id;
                    ordem.CarroId = atividade.Carro?.Id;
                    ordem.ClienteId = atividade.Carro?.Cliente?.Id;
                }
            }

            CarregarViewBags(ordem);
            return View(ordem);
        }

        [HttpPost]
        public IActionResult Create(OrdemServico ordem) {
            bool temErro = false;

            if (string.IsNullOrWhiteSpace(ordem.EtapaAtual))
            {
                ModelState.AddModelError("EtapaAtual", "O campo Etapa Atual é obrigatório.");
                temErro = true;
            }

            if (string.IsNullOrWhiteSpace(ordem.Prioridade))
            {
                ModelState.AddModelError("Prioridade", "O campo Prioridade é obrigatório.");
                temErro = true;
            }

            if (ordem.AtividadeId == 0)
            {
                ModelState.AddModelError("AtividadeId", "A Atividade vinculada é obrigatória.");
                temErro = true;
            }

            if (temErro)
            {
                CarregarViewBags(ordem);
                return View(ordem);
            }

            if (ordem.AtividadeId.HasValue && ordem.AtividadeId > 0)
            {
                var atividade = _context.Atividades
                    .Include(a => a.Funcionario)
                    .Include(a => a.Carro).ThenInclude(c => c.Cliente)
                    .Include(a => a.Etapa)
                    .FirstOrDefault(a => a.Id == ordem.AtividadeId);

                if (atividade == null)
                {
                    ModelState.AddModelError("AtividadeId", "Atividade não encontrada.");
                    CarregarViewBags(ordem);
                    return View(ordem);
                }

                ordem.FuncionarioId = atividade.Funcionario?.Id;
                ordem.CarroId = atividade.Carro?.Id;
                ordem.ClienteId = atividade.Carro?.Cliente?.Id;
                ordem.EtapaAtual = atividade.Etapa?.Nome ?? "N/A";
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
                .Include(o => o.Atividade).ThenInclude(a => a.Etapa)
                .FirstOrDefault(o => o.Id == id);

            if (ordem == null) return NotFound();

            CarregarViewBags(ordem);
            return View(ordem);
        }

        [HttpPost]
        public IActionResult Edit(OrdemServico ordem) {
            if (ordem == null || ordem.Id == 0)
                return BadRequest();

            var original = _context.OrdemServicos.FirstOrDefault(o => o.Id == ordem.Id);
            if (original == null)
                return NotFound();

            original.EtapaAtual = ordem.EtapaAtual;
            original.Prioridade = ordem.Prioridade;
            original.FuncionarioId = ordem.FuncionarioId;
            original.CarroId = ordem.CarroId;
            original.ClienteId = ordem.ClienteId;

            _context.SaveChanges();
            return RedirectToAction("Index");
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

            // Se não tiver Atividade, carrega dados manualmente
            if (ordem.Atividade == null)
            {
                ordem.FuncionarioId ??= 0;
                ordem.CarroId ??= 0;
                ordem.ClienteId ??= 0;

                ordem.Atividade = new Atividade
                {
                    Funcionario = _context.Funcionarios.Find(ordem.FuncionarioId),
                    Carro = _context.Carros
                        .Include(c => c.Cliente)
                        .FirstOrDefault(c => c.Id == ordem.CarroId),
                    Servico = null, // Se necessário, você pode adicionar campo ServicoId na Ordem
                    Etapa = null
                };
            }

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

            // Se não houver atividade, busca os dados manualmente
            if (ordem.Atividade == null)
            {
                ordem.Atividade = new Atividade
                {
                    Funcionario = _context.Funcionarios.Find(ordem.FuncionarioId),
                    Carro = _context.Carros
                        .Include(c => c.Cliente)
                        .FirstOrDefault(c => c.Id == ordem.CarroId),
                    Servico = null,
                    Etapa = null
                };
            }

            var doc = new RelatorioOrdemServicoPdf(ordem);
            var pdf = doc.GeneratePdf();

            return File(pdf, "application/pdf", $"ordem_servico_{id}.pdf");
        }

    }
}
