using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoAutomotiva.Controllers
{
    public class CarroController : Controller
    {
        private readonly AppDbContext _context;

        public CarroController(AppDbContext context) {
            _context = context;
        }

        // Tela de listagem dos Carros
        public IActionResult Index(string busca = null) {
            var carros = _context.Carros
                .Include(c => c.Cliente) // Inclui os Clientes para exibir os dados relacionados
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                var buscaUpper = busca.ToUpper();
                carros = carros.Where(c =>
                    c.IdCarro.Contains(buscaUpper) ||
                    c.Modelo.Contains(buscaUpper) ||
                    c.Cliente.Nome.Contains(buscaUpper));
            }

            return View(carros.ToList());
        }

        // Tela de Criar Carro
        public IActionResult Create() {
            ViewBag.Clientes = _context.Clientes.ToList();
            return View();
        }

        // Método POST para Criar Carro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Carro carro) {


            bool temErro = false;

            if (string.IsNullOrEmpty(carro.IdCarro))
            {
                ModelState.AddModelError("Carro", "Campo Código Carro Obrigatório.");
            }

            if (string.IsNullOrEmpty(carro.Modelo))
            {
                ModelState.AddModelError("Modelo", "Campo Modelo Obrigatório");
            }
            else
            {
                carro.Modelo = carro.Modelo.ToUpper();
            }

            if (string.IsNullOrEmpty(carro.Cor))
            {
                ModelState.AddModelError("Cor", "Campo Cor Obrigatório");
            }
            else
            {
                carro.Cor = carro.Cor.ToUpper();
            }

            var cliente = _context.Clientes.Find(carro.ClienteId);

            if (cliente !=null)
            {
                carro.Cliente = cliente;
            }

            // Verifica se o cliente foi selecionado e valida
            if (carro.ClienteId == 0)
            {
                ModelState.AddModelError("Cliente", "Campo Cliente Obrigatório");
            }
            else
            {
                _context.Carros.Add(carro);
                _context.SaveChanges();
                return RedirectToAction("Index"); // Redireciona para a lista de carros
            }          
            
            if (temErro)
            {
                ViewBag.Clientes = _context.Clientes.ToList();
                return View(carro);
            }
           
            return View(carro);
        }

        // Tela de Editar Carro
        public IActionResult Edit(int id) {
            var carro = _context.Carros.Include(c => c.Cliente).FirstOrDefault(c => c.Id == id);
            if (carro == null)
                return NotFound();

            ViewBag.Clientes = _context.Clientes.ToList();
            return View(carro);
        }

        // Método POST para Editar Carro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Carro carro) {
            if (ModelState.IsValid)
            {
                _context.Carros.Update(carro);
                _context.SaveChanges();
                return RedirectToAction("Index"); // Redireciona para a lista de carros
            }

            ViewBag.Clientes = _context.Clientes.ToList();
            return View(carro);
        }

        // Tela de Excluir Carro
        public IActionResult Delete(int id) {
            var carro = _context.Carros.Include(c => c.Cliente).FirstOrDefault(c => c.Id == id);
            if (carro == null)
                return NotFound();

            return View(carro);
        }

        // Método POST para Excluir Carro
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id) {
            var carro = _context.Carros.Find(id);
            if (carro == null)
                return NotFound();

            _context.Carros.Remove(carro);
            _context.SaveChanges();
            return RedirectToAction("Index"); // Redireciona para a lista de carros
        }
    }
}
