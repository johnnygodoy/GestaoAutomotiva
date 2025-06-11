using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using GestaoAutomotiva.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            int pageSize = 5; // Quantidade de itens por página

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
        public async Task<IActionResult> Create(Funcionario funcionario) {
            if (ModelState.IsValid)
            {
                funcionario.Nome = funcionario.Nome.ToUpper();
                funcionario.Especialidade = funcionario.Especialidade.ToUpper();
                funcionario.Status = funcionario.Status.ToUpper();

                _context.Funcionarios.Add(funcionario);

                bool salvo = await RetryHelper.TentarSalvarAsync(async () =>
                {
                    await _context.SaveChangesAsync();
                });

                if (!salvo)
                {
                    ModelState.AddModelError("", "Erro ao salvar: o banco estava ocupado. Tente novamente.");
                    return View(funcionario);
                }

                TempData["Mensagem"] = $"Funcionário {funcionario.Nome} foi adicionado com sucesso.";
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
            TempData["Mensagem"] = $"Funcionário {funcionario.Nome} foi editado com sucesso.";
            return View(funcionario);

        }
        [HttpPost]
        public async Task<IActionResult> Edit(Funcionario funcionario) {
            if (ModelState.IsValid)
            {
                funcionario.Nome = funcionario.Nome.ToUpper();
                funcionario.Especialidade = funcionario.Especialidade.ToUpper();
                funcionario.Status = funcionario.Status.ToUpper();

                _context.Funcionarios.Update(funcionario);

                bool salvo = await RetryHelper.TentarSalvarAsync(async () =>
                {
                    await _context.SaveChangesAsync();
                });

                if (!salvo)
                {
                    ModelState.AddModelError("", "Erro ao salvar: o banco estava ocupado. Tente novamente.");
                    return View(funcionario);
                }

                TempData["Mensagem"] = $"Funcionário {funcionario.Nome} foi editado com sucesso.";
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
            TempData["Mensagem"] = $"Funcionário {funcionario.Nome} foi excluído com sucesso.";
            return View(funcionario);        
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var funcionario = await _context.Funcionarios.FirstOrDefaultAsync(f => f.Id == id);
            if (funcionario == null)
            {
                return NotFound();
            }

            _context.Funcionarios.Remove(funcionario);

            bool salvo = await RetryHelper.TentarSalvarAsync(async () =>
            {
                await _context.SaveChangesAsync();
            });

            if (!salvo)
            {
                TempData["Erro"] = "Erro ao excluir o funcionário. O banco estava ocupado. Tente novamente.";
                return RedirectToAction("Index");
            }

            TempData["Mensagem"] = "Funcionário excluído com sucesso.";
            return RedirectToAction("Index");
        }

    }
}
