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

        public async Task<IActionResult> Index() {
            var lista = await _context.Motors.ToListAsync();
            return View(lista);
        }

        public IActionResult Create() {
            CarregarClientes();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Motor motor) {
            bool temErro = false;

            if (string.IsNullOrWhiteSpace(motor.Nome))
            {
                ModelState.AddModelError("Nome", "O campo Nome do motor é obrigatório.");
                temErro = true;
            }

            if (!Enum.IsDefined(typeof(StatusMotor), motor.Status))
            {
                ModelState.AddModelError("Status", "Selecione um status válido.");
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
