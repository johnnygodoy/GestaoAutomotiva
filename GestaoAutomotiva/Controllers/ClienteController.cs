using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoAutomotiva.Controllers
{
    public class ClienteController : Controller
    {
        private readonly AppDbContext _context;

        public ClienteController(AppDbContext context) {
            _context = context;
        }

        // Tela de listagem dos Clientes
        public IActionResult Index(string busca = null) {

            ViewData["Busca"] = busca;

            // Obtém todos os clientes inicialmente
            var clientes = _context.Clientes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
            {

                var buscaUpper = busca.ToUpper();

                clientes = clientes.Where(c => c.Nome.ToUpper().Contains(buscaUpper));
            }

            return View(clientes.ToList());
        }

        // Tela de Criar Cliente
        public IActionResult Create() {
            // Passa um modelo vazio para a view
            return View(new Cliente());
        }

        // Método POST para Criar Cliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Cliente cliente) {

            bool temErro = false;

            if (string.IsNullOrEmpty(cliente.Nome))
            {
                ModelState.AddModelError("Nome", "O campo nome é obrigatório.");
                temErro = true;
            }
            else
            {
                cliente.Nome = cliente.Nome.ToUpper();
            }

            if (string.IsNullOrEmpty(cliente.Telefone))
            {
                ModelState.AddModelError("Telefone", "O campo Telefone é obrigatório.");
                temErro = true;
            }

            if (string.IsNullOrEmpty(cliente.Endereco))
            {
                ModelState.AddModelError("Endereco", "O campo Endereço é obrigatório.");
                temErro = true;
            }
            else
            {
                cliente.Endereco = cliente.Endereco.ToUpper();

            }

            if (string.IsNullOrEmpty(cliente.CPF))
            {
                ModelState.AddModelError("CPF", "O campo CPF é obrigatório.");
                temErro = true;
            }

            if (temErro)
            {
                return View(cliente);
            }

            _context.Clientes.Add(cliente);
            _context.SaveChanges();
            return RedirectToAction("Index"); // Redireciona para a lista de clientes

        }

        // Tela de Editar Cliente
        public IActionResult Edit(int id) {
            var cliente = _context.Clientes.FirstOrDefault(c => c.Id == id);
            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        // Método POST para Editar Cliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Cliente cliente) {


            bool temErro = false;

            if (string.IsNullOrEmpty(cliente.Nome))
            {
                ModelState.AddModelError("Nome", "O campo nome é obrigatório.");
                temErro = true;
            }
            else
            {
                cliente.Nome = cliente.Nome.ToUpper();
            }

            if (string.IsNullOrEmpty(cliente.Telefone))
            {
                ModelState.AddModelError("Telefone", "O campo Telefone é obrigatório.");
                temErro = true;
            }

            if (string.IsNullOrEmpty(cliente.Endereco))
            {
                ModelState.AddModelError("Endereco", "O campo Endereço é obrigatório.");
                temErro = true;
            }
            else
            {
                cliente.Endereco = cliente.Endereco.ToUpper();

            }

            if (string.IsNullOrEmpty(cliente.CPF))
            {
                ModelState.AddModelError("CPF", "O campo CPF é obrigatório.");
                temErro = true;
            }

            if (temErro)
            {
                return View(cliente);
            }

            _context.Clientes.Update(cliente);
            _context.SaveChanges();
            return RedirectToAction("Index"); // Redireciona para a lista de clientes

        }

        // Tela de Excluir Cliente
        public IActionResult Delete(int id) {
            var cliente = _context.Clientes.FirstOrDefault(c => c.Id == id);
            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        // Método POST para Excluir Cliente
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id) {
            var cliente = _context.Clientes.Find(id);
            if (cliente == null)
                return NotFound();

            _context.Clientes.Remove(cliente);
            _context.SaveChanges();
            return RedirectToAction("Index"); // Redireciona para a lista de clientes
        }
    }
}
