using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace GestaoAutomotiva.Controllers
{
    public class FuncionarioController:Controller
    {
        private readonly AppDbContext _context;

        public FuncionarioController(AppDbContext context) {
            _context = context;
        }


        [HttpGet]
        public IActionResult Index(string busca = null, int page = 1) {
            ViewData["Busca"] = busca;

            int pageSize = 10; // Quantidade de itens por página

            // Obtendo a lista de funcionários
            var funcionarios = _context.Funcionarios.AsQueryable();

            // Aplicando filtro de busca (se houver)
            if (!string.IsNullOrWhiteSpace(busca))
            {
                var buscaUpper = busca.ToUpper();

                funcionarios = funcionarios.Where(f =>
                    f.Nome.Contains(buscaUpper) ||
                    f.Status.Contains(buscaUpper) ||
                    f.Especialidade.Contains(buscaUpper));
            }

            // Total de funcionários encontrados
            var totalRegistros = funcionarios.Count();

            // Calculando o total de páginas
            var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)pageSize);

            // Pegando a página solicitada e aplicando Skip e Take
            var funcionariosPaginados = funcionarios
                .Skip((page - 1) * pageSize) // Pular os itens da página anterior
                .Take(pageSize) // Pegar o número de itens da página atual
                .ToList();

            // Passando os dados para a View
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PaginaAtual = page;
            ViewBag.BuscaNome = busca;

            return View(funcionariosPaginados);
        }



        [HttpGet]
        public IActionResult Create() { 
        return View();
        }

        [HttpPost]
        public IActionResult Create(Funcionario funcionario) {

            if (ModelState.IsValid)
            {
                funcionario.Nome = funcionario.Nome.ToUpper();
                funcionario.Especialidade = funcionario.Especialidade.ToUpper();
                funcionario.Status = funcionario.Status.ToUpper();               

                _context.Funcionarios.Add(funcionario);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(funcionario);        
        }

        [HttpGet]
        public IActionResult Edit(int id) { 
        
        var funcionario = _context.Funcionarios.FirstOrDefault(x => x.Id == id);

            if (funcionario == null) {

                return NotFound();
            }

            return View(funcionario);

        }

        [HttpPost]
        public IActionResult Edit(Funcionario funcionario) {

            if (ModelState.IsValid) {

                funcionario.Nome = funcionario.Nome.ToUpper();
                funcionario.Especialidade = funcionario.Especialidade.ToUpper();
                funcionario.Status = funcionario.Status.ToUpper();

                _context.Funcionarios.Update(funcionario);
                _context.SaveChanges();
                return RedirectToAction("Index");
            
            }
            return View(funcionario);
        }

        [HttpGet]
        public IActionResult Delete(int id) { 
        
        var funcionario = _context.Funcionarios.FirstOrDefault(x=> x.Id == id);
            if (funcionario == null)
            {
                return NotFound();
            }

            return View(funcionario);        
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id) {

            var funcionario = _context.Funcionarios.FirstOrDefault(y => y.Id == id);
            if (funcionario == null)
            {
                return NotFound();
            }
            _context.Funcionarios.Remove(funcionario);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
