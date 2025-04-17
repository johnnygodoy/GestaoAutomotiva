using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
                .Include(c => c.Cliente)
                .OrderByDescending(c => c.Id)             
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
            var carro = new Carro
            {
                Cliente = new Cliente() // ✅ evita NullReferenceException na View
            };

            ViewBag.Carros = _context.Carros.ToList();
            return View(carro);
        }


        // Método POST para Criar Carro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Carro carro) {
            bool temErro = false;

            if (string.IsNullOrEmpty(carro.IdCarro))
            {
                ModelState.AddModelError("IdCarro", "Campo Código Carro é obrigatório.");
                temErro = true;
            }

            if (string.IsNullOrEmpty(carro.Modelo))
            {
                ModelState.AddModelError("Modelo", "Campo Modelo é obrigatório.");
                temErro = true;
            }
            else
            {
                carro.Modelo = carro.Modelo.ToUpper();
            }

            if (string.IsNullOrEmpty(carro.Cor))
            {
                ModelState.AddModelError("Cor", "Campo Cor é obrigatório.");
                temErro = true;
            }
            else
            {
                carro.Cor = carro.Cor.ToUpper();
            }

            // Validar Cliente
            if (carro.Cliente == null)
            {
                ModelState.AddModelError("Cliente", "Os dados do cliente são obrigatórios.");
                temErro = true;
            }
            else
            {
                if (string.IsNullOrEmpty(carro.Cliente.Nome))
                {
                    ModelState.AddModelError("Cliente.Nome", "Nome do cliente é obrigatório.");
                    temErro = true;
                }

                if (string.IsNullOrEmpty(carro.Cliente.Endereco))
                {
                    ModelState.AddModelError("Cliente.Endereco", "Endereço do cliente é obrigatório.");
                    temErro = true;
                }

                if (string.IsNullOrEmpty(carro.Cliente.Telefone))
                {
                    ModelState.AddModelError("Cliente.Telefone", "Telefone do cliente é obrigatório.");
                    temErro = true;
                }

                if (string.IsNullOrEmpty(carro.Cliente.CPF))
                {
                    ModelState.AddModelError("Cliente.CPF", "CPF do cliente é obrigatório.");
                    temErro = true;
                }
            }

            if (temErro)
            {
                return View(carro);
            }

            // Salvar Cliente e Carro
            _context.Clientes.Add(carro.Cliente);
            _context.SaveChanges(); // cliente.Id gerado aqui

            carro.ClienteId = carro.Cliente.Id;
            _context.Carros.Add(carro);
            _context.SaveChanges();

            return RedirectToAction("Index");
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
            bool temErro = false;

            if (string.IsNullOrEmpty(carro.IdCarro))
            {
                ModelState.AddModelError("IdCarro", "O campo Código do Carro é obrigatório.");
                temErro = true;
            }

            if (string.IsNullOrEmpty(carro.Modelo))
            {
                ModelState.AddModelError("Modelo", "O campo Modelo é obrigatório.");
                temErro = true;
            }
            else
            {
                carro.Modelo = carro.Modelo.ToUpper();
            }

            if (string.IsNullOrEmpty(carro.Cor))
            {
                ModelState.AddModelError("Cor", "O campo Cor é obrigatório.");
                temErro = true;
            }
            else
            {
                carro.Cor = carro.Cor.ToUpper();
            }

            if (carro.Cliente == null)
            {
                ModelState.AddModelError("Cliente", "Dados do cliente obrigatórios.");
                temErro = true;
            }
            else
            {
                if (string.IsNullOrEmpty(carro.Cliente.Nome))
                {
                    ModelState.AddModelError("Cliente.Nome", "Nome do cliente é obrigatório.");
                    temErro = true;
                }

                if (string.IsNullOrEmpty(carro.Cliente.Telefone))
                {
                    ModelState.AddModelError("Cliente.Telefone", "Telefone é obrigatório.");
                    temErro = true;
                }

                if (string.IsNullOrEmpty(carro.Cliente.Endereco))
                {
                    ModelState.AddModelError("Cliente.Endereco", "Endereço é obrigatório.");
                    temErro = true;
                }

                if (string.IsNullOrEmpty(carro.Cliente.CPF))
                {
                    ModelState.AddModelError("Cliente.CPF", "CPF é obrigatório.");
                    temErro = true;
                }
            }

            if (temErro)
            {
                ViewBag.Carros = _context.Carros.ToList();
                return View(carro);
            }

            // Atualiza o cliente primeiro
            var clienteExistente = _context.Clientes.Find(carro.ClienteId);
            if (clienteExistente != null)
            {
                clienteExistente.Nome = carro.Cliente.Nome.ToUpper();
                clienteExistente.Endereco = carro.Cliente.Endereco.ToUpper();
                clienteExistente.Telefone = carro.Cliente.Telefone;
                clienteExistente.CPF = carro.Cliente.CPF;

                _context.Clientes.Update(clienteExistente);
            }

            // Atualiza o carro
            _context.Carros.Update(carro);
            _context.SaveChanges();

            return RedirectToAction("Index");
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
