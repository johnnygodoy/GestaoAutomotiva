using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GestaoAutomotiva.Controllers
{
    public class CarroController : Controller
    {
        private readonly AppDbContext _context;

        public CarroController(AppDbContext context) {
            _context = context;
        }

        // Tela de listagem dos Carros
        public IActionResult Index(string busca = null, int page = 1) {

            int pageSize = 10; // Quantidade de itens por página

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

            // Total de  Carros encontrados
            var totalRegistros = carros.Count();

            // Calculando o total de páginas
            var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)pageSize);

            // Pegando a página solicitada e aplicando Skip e Take
            var carrosPaginados = carros
                .Skip((page - 1) * pageSize) // Pular os itens da página anterior
                .Take(pageSize) // Pegar o número de itens da página atual
                .ToList();

            // Passando os dados para a View
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PaginaAtual = page;
            ViewBag.BuscaNome = busca;


            return View(carrosPaginados);
        }

        // Tela de Criar Carro
        public IActionResult Create() {
            ViewBag.Clientes = _context.Clientes.ToList();
            var carros = _context.Carros.ToList(); // Obtém todos os carros do banco de dados
            ViewBag.Carros = carros; // Passa a lista de carros para a View
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

            // Garantir que os dados não sejam null antes de passá-los para a view
            ViewBag.Carros = _context.Carros.ToList() ?? new List<Carro>();
            ViewBag.Clientes = _context.Clientes.ToList() ?? new List<Cliente>();

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
