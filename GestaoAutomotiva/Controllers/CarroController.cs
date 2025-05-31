using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace GestaoAutomotiva.Controllers
{
    public class CarroController : Controller
    {
        private readonly AppDbContext _context;

        public CarroController(AppDbContext context) {
            _context = context;
        }

        public IActionResult Index(string busca, string tipoManutencao, int page = 1) {
            // Função para remover acentos
            string RemoverAcentos(string texto) {
                if (string.IsNullOrWhiteSpace(texto)) return texto;
                var normalized = texto.Normalize(NormalizationForm.FormD);
                var charsSemAcento = normalized
                    .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
                return new string(charsSemAcento.ToArray()).ToLowerInvariant();
            }

            // Carregar todos os dados em memória (necessário para usar funções C# como RemoverAcentos)
            var lista = _context.Carros
                .Include(c => c.Cliente)
                .Include(c => c.Modelo)
                .Include(c => c.Acessorios).ThenInclude(a => a.Motor)
                .Include(c => c.Acessorios).ThenInclude(a => a.Cambio)
                .Include(c => c.Acessorios).ThenInclude(a => a.Suspensao)
                .Include(c => c.Acessorios).ThenInclude(a => a.RodasPneus)
                .Include(c => c.Acessorios).ThenInclude(a => a.Carroceria)
                .Include(c => c.Acessorios).ThenInclude(a => a.Capota)
                .ToList();

            // Aplicar filtro de busca textual (sem acentos)
            if (!string.IsNullOrWhiteSpace(busca))
            {
                string buscaNormalizada = RemoverAcentos(busca);

                lista = lista.Where(c =>
                    RemoverAcentos(c.IdCarro ?? "").Contains(buscaNormalizada) ||
                    RemoverAcentos(c.Modelo?.Nome ?? "").Contains(buscaNormalizada) ||
                    RemoverAcentos(c.Cor ?? "").Contains(buscaNormalizada) ||
                    RemoverAcentos(c.Cliente?.Nome ?? "").Contains(buscaNormalizada) ||
                    RemoverAcentos(c.Cliente?.CPF ?? "").Contains(buscaNormalizada) ||
                    RemoverAcentos(c.Cliente?.Telefone ?? "").Contains(buscaNormalizada) ||
                    RemoverAcentos(c.Acessorios?.Motor?.Nome ?? "").Contains(buscaNormalizada) ||
                    RemoverAcentos(c.Acessorios?.Cambio?.Descricao ?? "").Contains(buscaNormalizada) ||
                    RemoverAcentos(c.Acessorios?.Suspensao?.Descricao ?? "").Contains(buscaNormalizada) ||
                    RemoverAcentos(c.Acessorios?.RodasPneus?.Descricao ?? "").Contains(buscaNormalizada) ||
                    RemoverAcentos(c.Acessorios?.Carroceria?.Descricao ?? "").Contains(buscaNormalizada) ||
                    RemoverAcentos(c.Acessorios?.Capota?.Descricao ?? "").Contains(buscaNormalizada) ||
                    RemoverAcentos(c.TipoManutencao ?? "").Contains(buscaNormalizada)
                ).ToList();
            }

            // Aplicar filtro por tipo de manutenção
            if (!string.IsNullOrWhiteSpace(tipoManutencao))
            {
                lista = lista.Where(c => c.TipoManutencao == tipoManutencao).ToList();
            }

            // Paginação
            int pageSize = 10;
            int total = lista.Count();
            var carros = lista
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Manter valores nos filtros após busca
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)total / pageSize);
            ViewBag.PaginaAtual = page;
            ViewBag.BuscaNome = busca;
            ViewBag.TipoManutencao = tipoManutencao;

            return View(carros);
        }


        public IActionResult Create() {

            ViewBag.TiposManutencao = new SelectList(new[]
{
    new { Value = "Producao", Text = "Produção" },
    new { Value = "Revisao", Text = "Revisão" }
}, "Value", "Text");
            PopularViewBags();
            var carro = new Carro
            {
                Cliente = new Cliente(),
                Acessorios = new AcessoriosCarro()
            };
            return View(carro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Carro carro) {
            bool temErro = false;

            if (string.IsNullOrEmpty(carro.IdCarro))
            {
                ModelState.AddModelError("IdCarro", "Campo Código Carro é obrigatório.");
                temErro = true;
            }

            if (carro.ModeloId <= 0)
            {
                ModelState.AddModelError("ModeloId", "Campo Modelo é obrigatório.");
                temErro = true;
            }

            if (string.IsNullOrEmpty(carro.Cor))
            {
                ModelState.AddModelError("Cor", "Campo Cor é obrigatório.");
                temErro = true;
            }

            if (carro.Cliente == null ||
                string.IsNullOrWhiteSpace(carro.Cliente.Nome) ||
                string.IsNullOrWhiteSpace(carro.Cliente.Endereco) ||
                string.IsNullOrWhiteSpace(carro.Cliente.Telefone) ||
                string.IsNullOrWhiteSpace(carro.Cliente.CPF))
            {
                ModelState.AddModelError("Cliente", "Todos os campos do cliente são obrigatórios.");
                temErro = true;
            }

            var acessorios = carro.Acessorios;
            if (acessorios == null ||
                acessorios.CapotaId == null ||
                acessorios.CambioId == null ||
                acessorios.CarroceriaId == null ||
                acessorios.SuspensaoId == null ||
                acessorios.RodasPneusId == null ||
                acessorios.MotorId == null)
            {
                ModelState.AddModelError("Acessorios", "Todos os campos de acessórios são obrigatórios.");
                temErro = true;
            }

            if (temErro)
            {
                PopularViewBags();
                return View(carro);
            }

            _context.Clientes.Add(carro.Cliente);
            _context.SaveChanges();
            carro.ClienteId = carro.Cliente.Id;

            var novoAcessorio = new AcessoriosCarro
            {
                CapotaId = acessorios.CapotaId,
                CambioId = acessorios.CambioId,
                CarroceriaId = acessorios.CarroceriaId,
                SuspensaoId = acessorios.SuspensaoId,
                RodasPneusId = acessorios.RodasPneusId,
                MotorId = acessorios.MotorId
            };

            _context.AcessoriosCarros.Add(novoAcessorio);
            _context.SaveChanges();

            carro.AcessoriosCarroId = novoAcessorio.Id;
            carro.Cliente = null;
            carro.Acessorios = null;

            _context.Carros.Add(carro);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id) {
            var carro = _context.Carros
                .Include(c => c.Cliente)
                .Include(c => c.Modelo)
                .Include(c => c.Acessorios)
                .Include(c => c.Modelo)
                .FirstOrDefault(c => c.Id == id);

            if (carro == null)
                return NotFound();

            ViewBag.TiposManutencao = new SelectList(new[]
            {
        new { Value = "Producao", Text = "Produção" },
        new { Value = "Revisao", Text = "Revisão" }
    }, "Value", "Text", carro.TipoManutencao); // pré-selecionar valor atual

            PopularViewBags();
            return View(carro);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Carro carro) {
            if (id != carro.Id)
                return BadRequest();

            bool temErro = false;

            if (string.IsNullOrEmpty(carro.IdCarro))
            {
                ModelState.AddModelError("IdCarro", "O campo Código do Carro é obrigatório.");
                temErro = true;
            }

            if (carro.ModeloId == 0)
            {
                ModelState.AddModelError("ModeloId", "O campo Modelo é obrigatório.");
                temErro = true;
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

            if (string.IsNullOrEmpty(carro.TipoManutencao))
            {
                ModelState.AddModelError("TipoManutencao", "Tipo de Manutenção é obrigatório.");
                temErro = true;
            }
            else
            {
                carro.TipoManutencao = carro.TipoManutencao;

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

            if (carro.Acessorios == null)
            {
                ModelState.AddModelError("Acessorios", "Dados de acessórios obrigatórios.");
                temErro = true;
            }
            else
            {
                if (carro.Acessorios.MotorId == 0)
                {
                    ModelState.AddModelError("Acessorios.MotorId", "Motor é obrigatório.");
                    temErro = true;
                }

                if (carro.Acessorios.CambioId == 0)
                {
                    ModelState.AddModelError("Acessorios.CambioId", "Câmbio é obrigatório.");
                    temErro = true;
                }

                if (carro.Acessorios.SuspensaoId == 0)
                {
                    ModelState.AddModelError("Acessorios.SuspensaoId", "Suspensão é obrigatória.");
                    temErro = true;
                }
            }

            if (temErro)
            {
                PopularViewBags();
                return View(carro);
            }

            var carroExistente = _context.Carros
                .Include(c => c.Cliente)
                .Include(c => c.Acessorios)
                .FirstOrDefault(c => c.Id == id);

            if (carroExistente == null)
                return NotFound();

            // Atualiza dados principais do carro
            carroExistente.IdCarro = carro.IdCarro;
            carroExistente.ModeloId = carro.ModeloId;
            carroExistente.Cor = carro.Cor;

            // Atualiza dados do cliente
            carroExistente.Cliente.Nome = carro.Cliente.Nome;
            carroExistente.Cliente.Endereco = carro.Cliente.Endereco;
            carroExistente.Cliente.Telefone = carro.Cliente.Telefone;
            carroExistente.Cliente.CPF = carro.Cliente.CPF;

            // Atualiza acessórios
            carroExistente.Acessorios.MotorId = carro.Acessorios.MotorId;
            carroExistente.Acessorios.CambioId = carro.Acessorios.CambioId;
            carroExistente.Acessorios.SuspensaoId = carro.Acessorios.SuspensaoId;
            carroExistente.Acessorios.RodasPneusId = carro.Acessorios.RodasPneusId;
            carroExistente.Acessorios.CarroceriaId = carro.Acessorios.CarroceriaId;
            carroExistente.Acessorios.CapotaId = carro.Acessorios.CapotaId;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public IActionResult Delete(int id) {
            var carro = _context.Carros
                .Include(c => c.Cliente)
                .Include(c => c.Modelo)
                .Include(c => c.Acessorios)
                .FirstOrDefault(c => c.Id == id);

            if (carro == null)
                return NotFound();

            return View(carro);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id) {
            var carro = _context.Carros
                .Include(c => c.Cliente)
                .Include(c => c.Acessorios)
                .FirstOrDefault(c => c.Id == id);

            if (carro == null)
                return NotFound();

            _context.Clientes.Remove(carro.Cliente);
            _context.AcessoriosCarros.Remove(carro.Acessorios);
            _context.Carros.Remove(carro);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        private void PopularViewBags() {
            ViewBag.Modelos = new SelectList(_context.Modelos.ToList(), "Id", "Nome");
            ViewBag.Motores = new SelectList(_context.Motors.ToList(), "Id", "Nome");
            ViewBag.Cambios = new SelectList(_context.Cambios.ToList(), "Id", "Descricao");
            ViewBag.Suspensoes = new SelectList(_context.Suspensaos.ToList(), "Id", "Descricao");
            ViewBag.RodasPneus = new SelectList(_context.RodasPneus.ToList(), "Id", "Descricao");
            ViewBag.Carrocerias = new SelectList(_context.Carrocerias.ToList(), "Id", "Descricao");
            ViewBag.Capotas = new SelectList(_context.Capotas.ToList(), "Id", "Descricao");
        }


    }
}
