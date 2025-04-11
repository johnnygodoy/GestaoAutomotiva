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
        public IActionResult Index(string busca = null) {
            ViewData["Busca"] = busca;

            var funcionarios = _context.Funcionarios.AsQueryable();            

            if (!string.IsNullOrWhiteSpace(busca))
            {
                var buscaUpper = busca.ToUpper();

                funcionarios = funcionarios.Where(f =>
                f.Nome.Contains(buscaUpper) ||
                f.Status.Contains(buscaUpper) ||
                f.Especialidade.Contains(buscaUpper));
            }

            var funcUpper = funcionarios.ToList().Select(f =>
            {
                f.Nome.ToUpper();
                f.Status.ToUpper();
                f.Especialidade.ToUpper();
                return f;
            }).ToList();

            return View(funcUpper);
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
