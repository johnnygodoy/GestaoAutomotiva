using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using GestaoAutomotiva.Utils;
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
                .Include(c => c.Acessorios).ThenInclude(a => a.Roda)
                .Include(c => c.Acessorios).ThenInclude(a => a.Pneu)
                .Include(c => c.Acessorios).ThenInclude(a => a.SantoAntonio)
                .Include(c => c.Acessorios).ThenInclude(a => a.Carroceria)
                .Include(c => c.Acessorios).ThenInclude(a => a.Capota)
                .Include(c => c.Acessorios).ThenInclude(a => a.Escapamento)
                .Include(c => c.Acessorios).ThenInclude(a => a.Painel)
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
                    RemoverAcentos(c.Acessorios?.Roda?.Descricao ?? "").Contains(buscaNormalizada) ||
                    RemoverAcentos(c.Acessorios?.Pneu?.Descricao ?? "").Contains(buscaNormalizada) ||
                      RemoverAcentos(c.Acessorios?.SantoAntonio?.Descricao ?? "").Contains(buscaNormalizada) ||
                    RemoverAcentos(c.Acessorios?.Carroceria?.Descricao ?? "").Contains(buscaNormalizada) ||
                    RemoverAcentos(c.Acessorios?.Capota?.Descricao ?? "").Contains(buscaNormalizada) ||
                    RemoverAcentos(c.Acessorios?.Escapamento?.Descricao ?? "").Contains(buscaNormalizada) ||
                    RemoverAcentos(c.Acessorios?.Painel?.Descricao ?? "").Contains(buscaNormalizada) ||
                    RemoverAcentos(c.TipoManutencao ?? "").Contains(buscaNormalizada)
                ).ToList();
            }

            // Aplicar filtro por tipo de manutenção
            if (!string.IsNullOrWhiteSpace(tipoManutencao))
            {
                lista = lista.Where(c => c.TipoManutencao == tipoManutencao).ToList();
            }

            // Paginação
            int pageSize = 5;
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
                 new { Value = "Fabricação", Text = "Fabricação" },
                 new { Value = "Producao", Text = "Produção" },
                 new { Value = "Retrabalho", Text = "Retrabalho" },       
                 new { Value = "Revisao", Text = "Revisão" }
            }, "Value", "Text");

            PopularViewBags();
            var carro = new Carro
            {
                Cliente = new Cliente(),
                Acessorios = new AcessoriosCarro()
            };
            TempData["Mensagem"] = $"Carro {carro.Modelo} foi criado com sucesso.";
            return View(carro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Carro carro) {
            bool temErro = false;

            // Validação manual
            if (string.IsNullOrWhiteSpace(carro.IdCarro))
            {
                ModelState.AddModelError("IdCarro", "Campo Código Carro é obrigatório.");
                temErro = true;
            }

            if (carro.ModeloId <= 0)
            {
                ModelState.AddModelError("ModeloId", "Campo Modelo é obrigatório.");
                temErro = true;
            }

            if (string.IsNullOrWhiteSpace(carro.Cor))
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
                acessorios.CapotaId <= 0 ||
                acessorios.CambioId <= 0 ||
                acessorios.CarroceriaId <= 0 ||
                acessorios.SuspensaoId <= 0 ||
                acessorios.RodaId <= 0 ||
                acessorios.PneuId <= 0 ||
                acessorios.SantoAntonioId <= 0 ||
                acessorios.EscapamentoId <= 0 ||
                acessorios.PainelId <= 0)
                
            {
                ModelState.AddModelError("Acessorios", "Todos os campos de acessórios são obrigatórios.");
                temErro = true;
            }

            if (temErro)
            {
                PopularViewBags();
                return View(carro);
            }

            // 🔁 Salva cliente com retry
            _context.Clientes.Add(carro.Cliente);
            bool clienteSalvo = await RetryHelper.TentarSalvarAsync(async () =>
            {
                await _context.SaveChangesAsync();
            });

            if (!clienteSalvo)
            {
                TempData["Erro"] = "Erro ao salvar cliente. Tente novamente.";
                PopularViewBags();
                return View(carro);
            }

            carro.ClienteId = carro.Cliente.Id;


            // 🛡️ Validação de integridade referencial (confirma se os IDs existem)
            bool motorValido = true;
            if (acessorios.MotorId.HasValue)
            {
                motorValido = await _context.Motors.AnyAsync(m => m.Id == acessorios.MotorId.Value);
            }

            bool idsValidos =
                motorValido &&
                await _context.Cambios.AnyAsync(c => c.Id == acessorios.CambioId) &&
                await _context.Suspensaos.AnyAsync(s => s.Id == acessorios.SuspensaoId) &&
                await _context.Rodas.AnyAsync(r => r.Id == acessorios.RodaId) &&
                await _context.Pneus.AnyAsync(r => r.Id == acessorios.PneuId) &&
                await _context.SantoAntonios.AnyAsync(r => r.Id == acessorios.SantoAntonioId) &&
                await _context.Carrocerias.AnyAsync(c => c.Id == acessorios.CarroceriaId) &&
                await _context.Capotas.AnyAsync(c => c.Id == acessorios.CapotaId) &&
                await _context.Escapamentos.AnyAsync(c => c.Id == acessorios.EscapamentoId) &&
                await _context.Paineis.AnyAsync(c => c.Id == acessorios.PainelId);

            if (!idsValidos)
            {
                TempData["Erro"] = "Um ou mais itens de acessórios não foram encontrados no banco de dados. Verifique os valores selecionados.";
                PopularViewBags();
                return View(carro);
            }

            // 🔁 Salva acessórios com retry
            var novoAcessorio = new AcessoriosCarro
            {
                CapotaId = acessorios.CapotaId,
                CambioId = acessorios.CambioId,
                CarroceriaId = acessorios.CarroceriaId,
                SuspensaoId = acessorios.SuspensaoId,
                RodaId = acessorios.RodaId,
                PneuId = acessorios.PneuId,
                SantoAntonioId = acessorios.SantoAntonioId,
                EscapamentoId = acessorios.EscapamentoId,
                PainelId = acessorios.PainelId,
                MotorId = acessorios.MotorId,
                ModeloId = carro.ModeloId
            };

            _context.AcessoriosCarros.Add(novoAcessorio);
            bool acessoriosSalvo = await RetryHelper.TentarSalvarAsync(async () =>
            {
                await _context.SaveChangesAsync();
            });

            if (!acessoriosSalvo)
            {
                TempData["Erro"] = "Erro ao salvar acessórios. Tente novamente.";
                PopularViewBags();
                return View(carro);
            }

            carro.AcessoriosCarroId = novoAcessorio.Id;
            carro.Cliente = null;
            carro.Acessorios = null;

            // 🔁 Salva carro com retry
            _context.Carros.Add(carro);
            bool carroSalvo = await RetryHelper.TentarSalvarAsync(async () =>
            {
                await _context.SaveChangesAsync();
            });

            if (!carroSalvo)
            {
                TempData["Erro"] = "Erro ao salvar o carro. Tente novamente.";
                PopularViewBags();
                return View(carro);
            }

            TempData["Mensagem"] = $"Carro cadastrado com sucesso!";
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
                 new { Value = "Fabricação", Text = "Fabricação" },
                 new { Value = "Producao", Text = "Produção" },
                 new { Value = "Retrabalho", Text = "Retrabalho" },
                 new { Value = "Revisao", Text = "Revisão" }
    }, "Value", "Text", carro.TipoManutencao); // pré-selecionar valor atual

            PopularViewBags();
            TempData["Mensagem"] = $"Carro {carro.Modelo} foi editado com sucesso.";
            return View(carro);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Carro carro) {
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

                if (carro.Acessorios.EscapamentoId == 0)
                {
                    ModelState.AddModelError("Acessorios.EscapamentoId", "Escapamento é obrigatória.");
                    temErro = true;
                }

                if (carro.Acessorios.PainelId == 0)
                {
                    ModelState.AddModelError("Acessorios.PainelId", "Painel é obrigatória.");
                    temErro = true;
                }
            }

            if (temErro)
            {
                PopularViewBags();
                return View(carro);
            }

            var carroExistente = await _context.Carros
         .Include(c => c.Cliente)
         .Include(c => c.Acessorios)
         .FirstOrDefaultAsync(c => c.Id == id);

            if (carroExistente == null)
                return NotFound();

            // Atualiza dados principais do carro
            carroExistente.IdCarro = carro.IdCarro;
            carroExistente.ModeloId = carro.ModeloId;
            carroExistente.Cor = carro.Cor;
            carroExistente.TipoManutencao = carro.TipoManutencao;

            // Atualiza cliente
            carroExistente.Cliente.Nome = carro.Cliente.Nome;
            carroExistente.Cliente.Telefone = carro.Cliente.Telefone;
            carroExistente.Cliente.Endereco = carro.Cliente.Endereco;
            carroExistente.Cliente.CPF = carro.Cliente.CPF;

            // Atualiza acessórios
            carroExistente.Acessorios.MotorId = carro.Acessorios.MotorId;
            carroExistente.Acessorios.CambioId = carro.Acessorios.CambioId;
            carroExistente.Acessorios.SuspensaoId = carro.Acessorios.SuspensaoId;
            carroExistente.Acessorios.RodaId = carro.Acessorios.RodaId;
            carroExistente.Acessorios.PneuId = carro.Acessorios.PneuId;
            carroExistente.Acessorios.SantoAntonioId = carro.Acessorios.SantoAntonioId;
            carroExistente.Acessorios.CarroceriaId = carro.Acessorios.CarroceriaId;
            carroExistente.Acessorios.CapotaId = carro.Acessorios.CapotaId;
            carroExistente.Acessorios.EscapamentoId = carro.Acessorios.EscapamentoId;
            carroExistente.Acessorios.PainelId = carro.Acessorios.PainelId;
            carroExistente.Acessorios.ModeloId = carro.ModeloId;

            // 🔁 Salva com retry
            bool sucesso = await RetryHelper.TentarSalvarAsync(async () =>
            {
                await _context.SaveChangesAsync();
            });

            if (!sucesso)
            {
                TempData["Mensagem"] = "Erro ao salvar alterações. O banco estava ocupado. Tente novamente.";
                PopularViewBags();
                return View(carro);
            }

            TempData["Mensagem"] = $"Carro {carro.IdCarro} foi editado com sucesso.";
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
            TempData["Mensagem"] = $"Carro {carro.Modelo} foi excluído com sucesso.";
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

            TempData["Mensagem"] = $"Carro {carro.Modelo} foi excluído com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        private void PopularViewBags() {
            ViewBag.Modelos = new SelectList(_context.Modelos.ToList(), "Id", "Nome");
            ViewBag.Motores = new SelectList(_context.Motors.ToList(), "Id", "Nome");
            ViewBag.Cambios = new SelectList(_context.Cambios.ToList(), "Id", "Descricao");
            ViewBag.Suspensoes = new SelectList(_context.Suspensaos.ToList(), "Id", "Descricao");
            ViewBag.Rodas = new SelectList(_context.Rodas.ToList(), "Id", "Descricao");
            ViewBag.Pneus = new SelectList(_context.Pneus.ToList(), "Id", "Descricao");
            ViewBag.SantoAntonios = new SelectList(_context.SantoAntonios.ToList(), "Id", "Descricao");
            ViewBag.Carrocerias = new SelectList(_context.Carrocerias.ToList(), "Id", "Descricao");
            ViewBag.Capotas = new SelectList(_context.Capotas.ToList(), "Id", "Descricao");
            ViewBag.Escapamentos = new SelectList(_context.Escapamentos.ToList(), "Id", "Descricao");
            ViewBag.Paineis = new SelectList(_context.Paineis.ToList(), "Id", "Descricao");
        }


    }
}
