using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace GestaoAutomotiva.Controllers
{
    public class ServicoController : Controller
    {
        private readonly AppDbContext _context;

        public ServicoController(AppDbContext context) {
            _context = context;
        }
  

        [HttpGet]
        public IActionResult Create() {
            return View();
        }

        public IActionResult Index(string busca = null, int page = 1) {

            ViewData["Busca"] = busca;

            int pageSize = 10; // Quantidade de itens por página

            var servicos = _context.Servicos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                var buscaUpper = busca.ToUpper();
                // Filtrar por Descrição ou Tipo
                servicos = servicos.Where(s => s.Descricao.Contains(buscaUpper) || s.Tipo.Contains(buscaUpper));

            }

            // Total de servicos encontrados
            var totalRegistros = servicos.Count();

            // Calculando o total de páginas
            var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)pageSize);

            // Pegando a página solicitada e aplicando Skip e Take
            var servicosPaginados = servicos
                .Skip((page - 1) * pageSize) // Pular os itens da página anterior
                .Take(pageSize) // Pegar o número de itens da página atual
                .ToList();

            // Passando os dados para a View
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PaginaAtual = page;
            ViewBag.BuscaNome = busca;

            return View(servicosPaginados);
        }

        [HttpPost]
        public IActionResult Create(Servico servico) {

            if (ModelState.IsValid)
            {
                servico.Tipo = servico.Tipo.ToUpper();
                servico.Descricao = servico.Descricao.ToUpper();              

                _context.Servicos.Add(servico);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(servico);
        }

        public IActionResult Edit(int id) { 
        
        var servico = _context.Servicos.FirstOrDefault(x => x.Id == id);
            if (servico == null) return NotFound();
            return View(servico);
        }

        [HttpPost]
        public IActionResult Edit(Servico servico) {

            servico.Tipo = servico.Tipo.ToUpper();
            servico.Descricao = servico.Descricao.ToUpper();

            _context.Servicos.Update(servico);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id) { 
        
       var servico = _context.Servicos.FirstOrDefault(x => x.Id == id);

            if (servico == null) return NotFound(); return View(servico);

        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id) { 
        
        var servico = _context.Servicos.FirstOrDefault(f => f.Id == id);
            _context.Servicos.Remove(servico);

            _context.SaveChanges();

            return RedirectToAction("Index");
        
        }
    }
}
