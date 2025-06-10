using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using GestaoAutomotiva.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace GestaoAutomotiva.Controllers
{
    public class OrdemServicoController : Controller
    {
        private readonly AppDbContext _context;

        public OrdemServicoController(AppDbContext context) {
            _context = context;
        }

        private void CarregarViewBags(OrdemServico ordem = null) {
            ViewBag.Funcionarios = new SelectList(_context.Funcionarios, "Id", "Nome", ordem?.FuncionarioId);

            // Carrega os carros com cliente e modelo
            var carros = _context.Carros
                .Include(c => c.Cliente)
                .Include(c => c.Modelo)
                .Select(c => new
                {
                    c.Id,
                    Descricao = string.Concat(
                     c.Modelo.Nome ?? "Modelo",
                     " - ",
                     c.IdCarro ?? "ID",
                     " - ",
                     c.Cliente.Nome ?? "Cliente"
                     )

                })
                .ToList();

            ViewBag.Carros = new SelectList(carros, "Id", "Descricao", ordem?.CarroId);

            ViewBag.Clientes = new SelectList(_context.Clientes, "Id", "Nome", ordem?.ClienteId);
        }

        private IQueryable<OrdemServico> ObterOrdensCompletas() {
            return _context.OrdemServicos
                .Include(o => o.Atividade).ThenInclude(a => a.Funcionario)
                .Include(o => o.Atividade).ThenInclude(a => a.Carro).ThenInclude(c => c.Cliente)
                .Include(o => o.Atividade).ThenInclude(a => a.Carro).ThenInclude(c => c.Modelo)
                .Include(o => o.Atividade).ThenInclude(a => a.Servico)
                .Include(o => o.Atividade).ThenInclude(a => a.Etapa);
        }


        public IActionResult Index(string busca = null, string dataBusca = null, int page = 1) {
            ViewData["Busca"] = busca;
            ViewData["DataBusca"] = dataBusca;

            int pageSize = 5;
            var query = ObterOrdensCompletas().AsQueryable();

            // Filtro por texto
            if (!string.IsNullOrWhiteSpace(busca))
            {
                var buscaUpper = busca.Trim().ToUpper();

                query = query.Where(o =>
                    (o.Atividade != null &&
                        (
                            (o.Atividade.Funcionario.Nome ?? "").ToUpper().Contains(buscaUpper) ||
                            (o.Atividade.Carro.Modelo.Nome ?? "").ToUpper().Contains(buscaUpper) ||                         
                            (o.Atividade.Carro.Cliente.Nome ?? "").ToUpper().Contains(buscaUpper) ||
                            (o.Atividade.Etapa.Nome ?? "").ToUpper().Contains(buscaUpper) ||
                            o.Atividade.Servico.Id.ToString().Contains(buscaUpper)
                        )
                    ) ||
                    (o.Prioridade ?? "").ToUpper().Contains(buscaUpper) ||
                    o.Id.ToString().Contains(buscaUpper)
                );
            }


            // Filtro por data de abertura
            if (!string.IsNullOrWhiteSpace(dataBusca) && DateTime.TryParse(dataBusca, out DateTime dataConvertida))
            {
                query = query.Where(o => o.DataAbertura.Date == dataConvertida.Date);
            }

            // Paginação
            var totalRegistros = query.Count();
            var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)pageSize);

            var paginado = query
                .OrderByDescending(o => o.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Fallback para registros incompletos
            foreach (var ordem in paginado)
            {
                if (ordem.Atividade == null)
                {
                    ordem.FuncionarioId ??= 0;
                    ordem.CarroId ??= 0;
                    ordem.ClienteId ??= 0;

                    ordem.Atividade = new Atividade
                    {
                        Funcionario = _context.Funcionarios.Find(ordem.FuncionarioId),
                        Carro = _context.Carros
                            .Include(c => c.Cliente)
                            .Include(c => c.Modelo)
                            .FirstOrDefault(c => c.Id == ordem.CarroId),
                        Etapa = _context.Etapas.FirstOrDefault(e => e.Nome == ordem.EtapaAtual)
                    };
                }
            }

            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PaginaAtual = page;
            ViewBag.BuscaNome = busca;

            ViewBag.Funcionarios = new SelectList(_context.Funcionarios.ToList(), "Id", "Nome");
            ViewBag.Clientes = new SelectList(_context.Clientes.ToList(), "Id", "Nome");

            return View(paginado);
        }



        public IActionResult CriarOuEditar(int atividadeId) {
            var existente = _context.OrdemServicos.FirstOrDefault(o => o.AtividadeId == atividadeId);
            if (existente != null)
                return RedirectToAction("Edit", new { id = existente.Id });

            return RedirectToAction("Create", new { atividadeId });
        }

        public IActionResult Create(int? atividadeId) {
            OrdemServico ordem = new OrdemServico
            {
                DataAbertura = DateTime.Today,
                EtapaAtual = "N/A",
                Prioridade = "Normal"
            };

            if (atividadeId.HasValue)
            {
                var atividade = _context.Atividades
                    .Include(a => a.Funcionario)
                    .Include(a => a.Etapa)
                    .FirstOrDefault(a => a.Id == atividadeId.Value);

                if (atividade != null)
                {
                    // Carrega os dados completos do carro, incluindo Cliente e Modelo
                    atividade.Carro = _context.Carros
                        .Include(c => c.Cliente)
                        .Include(c => c.Modelo)
                        .FirstOrDefault(c => c.Id == atividade.CarroId);

                    // Carrega o serviço vinculado à atividade (caso ainda não esteja incluído)
                    atividade.Servico = _context.Servicos.FirstOrDefault(s => s.Id == atividade.ServicoId);

                    // Preenche os dados na nova ordem de serviço
                    ordem.AtividadeId = atividade.Id;
                    ordem.Atividade = atividade;
                    ordem.FuncionarioId = atividade.Funcionario?.Id;
                    ordem.CarroId = atividade.Carro?.Id;
                    ordem.ClienteId = atividade.Carro?.Cliente?.Id;
                    ordem.EtapaAtual = atividade.Etapa?.Nome ?? "N/A";
                }
            }

            CarregarViewBags(ordem);
            TempData["Mensagem"] = $"Ordem de serviço foi criado com sucesso.";
            return View(ordem);
        }


        [HttpPost]
        public IActionResult Create(OrdemServico ordem) {
            bool temErro = false;

            if (string.IsNullOrWhiteSpace(ordem.EtapaAtual))
            {
                ModelState.AddModelError("EtapaAtual", "O campo Etapa Atual é obrigatório.");
                temErro = true;
            }

            if (string.IsNullOrWhiteSpace(ordem.Prioridade))
            {
                ModelState.AddModelError("Prioridade", "O campo Prioridade é obrigatório.");
                temErro = true;
            }

            if (ordem.AtividadeId == 0)
            {
                ModelState.AddModelError("AtividadeId", "A Atividade vinculada é obrigatória.");
                temErro = true;
            }

            if (temErro)
            {
                CarregarViewBags(ordem);
                return View(ordem);
            }

            if (ordem.AtividadeId.HasValue && ordem.AtividadeId > 0)
            {
                var atividade = _context.Atividades
                    .Include(a => a.Funcionario)
                    .Include(a => a.Carro).ThenInclude(c => c.Cliente)
                    .Include(a => a.Etapa)
                    .FirstOrDefault(a => a.Id == ordem.AtividadeId);

                if (atividade == null)
                {
                    ModelState.AddModelError("AtividadeId", "Atividade não encontrada.");
                    CarregarViewBags(ordem);
                    return View(ordem);
                }

                ordem.FuncionarioId = atividade.Funcionario?.Id;
                ordem.CarroId = atividade.Carro?.Id;
                ordem.ClienteId = atividade.Carro?.Cliente?.Id;
                ordem.EtapaAtual = atividade.Etapa?.Nome ?? "N/A";
            }

            _context.OrdemServicos.Add(ordem);
            _context.SaveChanges();
            TempData["Mensagem"] = $"Ordem de serviço foi criado com sucesso.";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id) {
            var ordem = _context.OrdemServicos
                .Include(o => o.Atividade).ThenInclude(a => a.Funcionario)
                .Include(o => o.Atividade).ThenInclude(a => a.Carro).ThenInclude(c => c.Cliente)
                .Include(o => o.Atividade).ThenInclude(a => a.Carro).ThenInclude(c => c.Modelo)
                .Include(o => o.Atividade).ThenInclude(a => a.Servico)
                .Include(o => o.Atividade).ThenInclude(a => a.Etapa)
                .FirstOrDefault(o => o.Id == id);

            if (ordem == null) return NotFound();

            // Fallback se Atividade não estiver carregada
            if (ordem.Atividade == null)
            {
                ordem.FuncionarioId ??= 0;
                ordem.CarroId ??= 0;
                ordem.ClienteId ??= 0;

                ordem.Atividade = new Atividade
                {
                    Funcionario = _context.Funcionarios.Find(ordem.FuncionarioId),
                    Carro = _context.Carros
                        .Include(c => c.Cliente)
                        .Include(c => c.Modelo)
                        .FirstOrDefault(c => c.Id == ordem.CarroId),
                    Servico = _context.Servicos.FirstOrDefault(s => s.Id == ordem.Id),
                    Etapa = _context.Etapas.FirstOrDefault(e => e.Nome == ordem.EtapaAtual)
                };
            }

            CarregarViewBags(ordem);
            TempData["Mensagem"] = $"Ordem de serviço foi editado com sucesso.";
            return View(ordem);
        }


        [HttpPost]
        public IActionResult Edit(OrdemServico ordem) {
            if (ordem == null || ordem.Id == 0)
                return BadRequest();

            var original = _context.OrdemServicos.FirstOrDefault(o => o.Id == ordem.Id);
            if (original == null)
                return NotFound();

            original.EtapaAtual = ordem.EtapaAtual;
            original.Prioridade = ordem.Prioridade;
            original.FuncionarioId = ordem.FuncionarioId;
            original.CarroId = ordem.CarroId;
            original.ClienteId = ordem.ClienteId;
            original.Tarefas = ordem.Tarefas;
            original.Observacoes = ordem.Observacoes;

            _context.SaveChanges();
            TempData["Mensagem"] = $"Ordem de serviço foi editado com sucesso.";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id) {
            var ordem = _context.OrdemServicos
                .Include(o => o.Atividade).ThenInclude(a => a.Carro).ThenInclude(c => c.Cliente)
                .FirstOrDefault(o => o.Id == id);

            if (ordem == null) return NotFound();

            TempData["Mensagem"] = $"Ordem de serviço foi excluído com sucesso.";
            return View(ordem);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id) {
            var ordem = _context.OrdemServicos.Find(id);
            if (ordem == null) return NotFound();

            _context.OrdemServicos.Remove(ordem);
            _context.SaveChanges();

            TempData["Mensagem"] = $"Ordem de serviço foi excluído com sucesso.";
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id) {
            var ordem = _context.OrdemServicos
                .Include(o => o.Atividade).ThenInclude(a => a.Funcionario)
                .Include(o => o.Atividade).ThenInclude(a => a.Carro).ThenInclude(c => c.Cliente)
                .Include(o => o.Atividade).ThenInclude(a => a.Carro).ThenInclude(c => c.Modelo)
                .Include(o => o.Atividade).ThenInclude(a => a.Servico)
                .Include(o => o.Atividade).ThenInclude(a => a.Etapa)
                .FirstOrDefault(o => o.Id == id);

            if (ordem == null) return NotFound();

            // Fallback caso a Atividade não esteja populada corretamente
            if (ordem.Atividade == null)
            {
                ordem.FuncionarioId ??= 0;
                ordem.CarroId ??= 0;
                ordem.ClienteId ??= 0;

                ordem.Atividade = new Atividade
                {
                    Funcionario = _context.Funcionarios.Find(ordem.FuncionarioId),
                    Carro = _context.Carros
                        .Include(c => c.Cliente)
                        .Include(c => c.Modelo)
                        .FirstOrDefault(c => c.Id == ordem.CarroId),
                    Servico = _context.Servicos.FirstOrDefault(s => s.Id == ordem.Id),
                    Etapa = _context.Etapas.FirstOrDefault(e => e.Nome == ordem.EtapaAtual)
                };
            }

            return View(ordem);
        }



        public IActionResult Visualizar(int id) {
            return Details(id);
        }

        public IActionResult ExportarPdf(int id) {
            var ordem = _context.OrdemServicos
                .Include(o => o.Atividade).ThenInclude(a => a.Funcionario)
                .Include(o => o.Atividade).ThenInclude(a => a.Carro).ThenInclude(c => c.Cliente)
                .Include(o => o.Atividade).ThenInclude(a => a.Carro).ThenInclude(c => c.Modelo)
                .Include(o => o.Atividade).ThenInclude(a => a.Carro).ThenInclude(c => c.Acessorios).ThenInclude(a => a.Cambio)
                .Include(o => o.Atividade).ThenInclude(a => a.Carro).ThenInclude(c => c.Acessorios).ThenInclude(a => a.Carroceria)
                .Include(o => o.Atividade).ThenInclude(a => a.Carro).ThenInclude(c => c.Acessorios).ThenInclude(a => a.Capota)
                .Include(o => o.Atividade).ThenInclude(a => a.Carro).ThenInclude(c => c.Acessorios).ThenInclude(a => a.Motor)
                .Include(o => o.Atividade).ThenInclude(a => a.Carro).ThenInclude(c => c.Acessorios).ThenInclude(a => a.Suspensao)
                .Include(o => o.Atividade).ThenInclude(a => a.Carro).ThenInclude(c => c.Acessorios).ThenInclude(a => a.RodasPneus)
                .Include(o => o.Atividade).ThenInclude(a => a.Servico)
                .FirstOrDefault(o => o.Id == id);

            // Verificação e fallback
            if (ordem == null)
                return NotFound();

            if (ordem.Atividade == null)
            {
                ordem.FuncionarioId ??= 0;
                ordem.CarroId ??= 0;
                ordem.ClienteId ??= 0;

                ordem.Atividade = new Atividade
                {
                    Funcionario = _context.Funcionarios.Find(ordem.FuncionarioId),
                    Carro = _context.Carros
                        .Include(c => c.Cliente)
                        .Include(c => c.Modelo)
                        .Include(c => c.Acessorios)
                            .ThenInclude(a => a.Motor)
                        .Include(c => c.Acessorios)
                            .ThenInclude(a => a.Cambio)
                        .Include(c => c.Acessorios)
                            .ThenInclude(a => a.Suspensao)
                        .Include(c => c.Acessorios)
                            .ThenInclude(a => a.RodasPneus)
                        .Include(c => c.Acessorios)
                            .ThenInclude(a => a.Carroceria)
                        .Include(c => c.Acessorios)
                            .ThenInclude(a => a.Capota)
                        .FirstOrDefault(c => c.Id == ordem.CarroId),
                    Servico = _context.Servicos.FirstOrDefault(s => s.Id == ordem.Id),
                    Etapa = _context.Etapas.FirstOrDefault(e => e.Nome == ordem.EtapaAtual)

                };
            }

            var doc = new RelatorioOrdemServicoPdf(ordem);
            var pdf = doc.GeneratePdf();

            return File(pdf, "application/pdf", $"ordem_servico_{id}.pdf");
        }


    }
}
