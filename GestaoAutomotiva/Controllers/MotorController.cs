using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoAutomotiva.Controllers
{
    public class MotorController : Controller
    {
        private readonly AppDbContext _context;

        public MotorController(AppDbContext context) {
            _context = context;
        }

        private void CarregarClientes() {
            ViewBag.Clientes = new SelectList(_context.Clientes.OrderBy(c => c.Nome), "Id", "Nome");
        }

        [HttpGet]
        public IActionResult Index(string busca = null, int page = 1) {
            ViewData["Busca"] = busca;

            int pageSize = 10; // Quantidade de itens por página

            var motores = _context.Motors.AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                var buscaUpper = busca.ToUpper();

                motores = motores.Where(m =>
                    m.Nome.ToUpper().Contains(buscaUpper) ||
                    m.PlacaVeiculoDoador.ToUpper().Contains(buscaUpper) ||
                    m.NumeroMotor.ToUpper().Contains(buscaUpper) ||
                    m.Status.ToString().ToUpper().Contains(buscaUpper));
            }

            int totalRegistros = motores.Count();
            int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)pageSize);

            var motoresPaginados = motores
                .OrderBy(m => m.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PaginaAtual = page;
            ViewBag.BuscaNome = busca;

            return View(motoresPaginados);
        }


        public IActionResult Create() {
            CarregarClientes();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Motor motor) {

            bool temErro = false;

            if (motor.Nome ==null)
            {
                ModelState.AddModelError("Modelo", "Nome do motor obrigatório.");
                temErro = true;
            }


            if (motor.Status == null)
            {
                ModelState.AddModelError("status", "Status motor obrigatório.");
                temErro = true;
            }

            if (temErro)
            {
                CarregarClientes();
                return View(motor);
            }

            motor.Nome = motor.Nome?.ToUpper();
            motor.PlacaVeiculoDoador = string.IsNullOrWhiteSpace(motor.PlacaVeiculoDoador) ? "A DEFINIR" : motor.PlacaVeiculoDoador.ToUpper();
            motor.NumeroMotor = string.IsNullOrWhiteSpace(motor.NumeroMotor) ? "A DEFINIR" : motor.NumeroMotor.ToUpper();
            motor.Observacoes = string.IsNullOrWhiteSpace(motor.Observacoes) ? "A DEFINIR" : motor.Observacoes.ToUpper();

            try
            {
                _context.Motors.Add(motor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError("", "Erro ao salvar motor: " + msg);
                return View(motor);
            }
        }



        public async Task<IActionResult> Edit(int id) {
            var motor = await _context.Motors.FindAsync(id);
            if (motor == null) return NotFound();
            CarregarClientes();
            return View(motor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Motor motor) {
            if (id != motor.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existente = await _context.Motors.FindAsync(id);
                if (existente == null) return NotFound();

                existente.Nome = motor.Nome?.ToUpper();
                existente.PlacaVeiculoDoador = motor.PlacaVeiculoDoador?.ToUpper();
                existente.NumeroMotor = motor.NumeroMotor?.ToUpper();
                existente.Observacoes = motor.Observacoes?.ToUpper();
                existente.Status = motor.Status;
                existente.ClienteId = motor.ClienteId;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            CarregarClientes();
            return View(motor);
        }


        public async Task<IActionResult> Delete(int id) {
            var motor = await _context.Motors.FindAsync(id);
            if (motor == null) return NotFound();
            return View(motor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var motor = await _context.Motors.FindAsync(id);
            if (motor != null)
            {
                _context.Motors.Remove(motor);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
