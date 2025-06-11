using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using GestaoAutomotiva.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace GestaoAutomotiva.Controllers
{
    public class AtividadeController : Controller
    {
        private readonly AppDbContext _context;

        public AtividadeController(AppDbContext context) {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create() {
            PreencherDropdowns(); // apenas chama        

            return View();
        }

        public async Task<IActionResult> Finalizar(int id) {
            var atividade = _context.Atividades.Find(id);
            if (atividade == null) return NotFound();

            if (atividade.DataPrevista.HasValue && atividade.DataPrevista.Value < DateTime.Today)
            {
                var diasAtraso = (DateTime.Today - atividade.DataPrevista.Value).Days;
                atividade.Status = $"Finalizado com atraso de {diasAtraso} dia{(diasAtraso > 1 ? "s" : "")}";
            }
            else
            {
                atividade.Status = "Finalizado";
            }

            atividade.Cor = DefinirCorStatus(atividade.Status);

            var salvo = await RetryHelper.TentarSalvarAsync(async () =>
            {
                await _context.SaveChangesAsync();
            });

            if (!salvo)
            {
                TempData["Erro"] = "Erro ao finalizar a atividade. Tente novamente.";
                return RedirectToAction("Index");
            }

            var historicoSalvo = await RegistrarHistorico(atividade, "Finalizado");

            if (!historicoSalvo)
            {
                TempData["Erro"] = "Atividade finalizada, mas falha ao registrar o histórico.";
            }
            else
            {
                TempData["Mensagem"] = "Atividade foi finalizada com sucesso.";
            }

            return RedirectToAction("Index");
        }


        private string DefinirCorStatus(string status) {
            return status switch
            {
                var s when s.Contains("Finalizado com atraso") => "#e74c3c",
                "Finalizado" => "#2ecc71",
                "Parado" => "#f1c40f",
                "Nao Iniciado" => "#e67e22",
                "Em Andamento" => "#3498db",
                "Cancelado" => "#e74c3c",
                _ => "#ffffff" // branco ou neutro
            };
        }



        public IActionResult Index(string busca = null, string dataBusca = null, int page = 1) {
            ViewData["Busca"] = busca;
            ViewData["DataBusca"] = dataBusca;

            int pageSize = 5; // Quantidade de itens por página

            // Inicializando a query de atividades
            var atividades = _context.Atividades
                .Include(a => a.Funcionario)
                .Include(a => a.Servico)
                .Include(a => a.Carro)
                .ThenInclude(c => c.Cliente)
                .Include(a => a.Carro)
                .ThenInclude(c => c.Modelo)
                 .Include(a => a.Etapa)
                .OrderByDescending(c => c.Id)
                .AsQueryable();

            // Busca por texto (funcionário, carro, modelo, serviço, status, etc.)
            if (!string.IsNullOrWhiteSpace(busca))
            {
                var buscaUpper = busca.ToUpper(); // Converte a busca para maiúsculas

                atividades = atividades.Where(a =>
                    a.Carro.Cliente.Nome.ToUpper().Contains(buscaUpper) ||  // Cliente
                    a.Carro.Modelo.Nome.ToUpper().Contains(buscaUpper) ||        // Modelo do carro
                    a.Funcionario.Nome.ToUpper().Contains(buscaUpper) ||    // Nome do funcionário
                    a.Servico.Descricao.ToUpper().Contains(buscaUpper) ||   // Descrição do serviço
                    a.Carro.IdCarro.ToUpper().Contains(buscaUpper) ||
                    a.Status.ToUpper().Contains(buscaUpper));               // Status da atividade
            }

            // Busca por data
            if (!string.IsNullOrWhiteSpace(dataBusca) && DateTime.TryParse(dataBusca, out DateTime dataBuscaConvertida))
            {
                atividades = atividades.Where(a =>
                    a.DataInicio == dataBuscaConvertida.Date ||  // Comparar apenas as datas (ignorando a hora)
                    a.DataPrevista == dataBuscaConvertida.Date); // Comparar apenas as datas (ignorando a hora)
            }

            // Total de atividades encontrados
            var totalRegistros = atividades.Count();

            // Calculando o total de páginas
            var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)pageSize);

            // Pegando a página solicitada e aplicando Skip e Take
            var atividadesPaginados = atividades
                .Skip((page - 1) * pageSize) // Pular os itens da página anterior
                .Take(pageSize) // Pegar o número de itens da página atual
                .ToList();

            // Passando os dados para a View
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PaginaAtual = page;
            ViewBag.BuscaNome = busca;


            // Retorna a lista de atividades com as buscas aplicadas
            return View(atividadesPaginados);
        }


        [HttpPost]
        public async Task<IActionResult> Create(Atividade atividade) {
            var funcionarios = _context.Funcionarios.ToList();
            var servicos = _context.Servicos.ToList();

            bool temErro = false;

            // Validação básica
            if (atividade.FuncionarioId == 0)
            {
                ModelState.AddModelError("FuncionarioId", "Selecione um funcionário.");
                temErro = true;
            }

            if (atividade.ServicoId == 0)
            {
                ModelState.AddModelError("ServicoId", "Selecione um serviço.");
                temErro = true;
            }

            var carro = _context.Carros
                .Include(c => c.Modelo)
                .Include(c => c.Cliente)
                .FirstOrDefault(c => c.Id == atividade.CarroId);

            if (carro == null)
            {
                ModelState.AddModelError("CarroId", "Carro não encontrado.");
                temErro = true;
            }

            if (!atividade.DataInicio.HasValue)
            {
                ModelState.AddModelError("DataInicio", "A data de início é obrigatória.");
                temErro = true;
            }

            if (temErro)
            {
                PreencherDropdowns();
                return View(atividade);
            }

            atividade.Carro = carro;
            atividade.Status = "Em Andamento";
            atividade.Cor = DefinirCorStatus(atividade.Status);

            var servico = servicos.FirstOrDefault(s => s.Id == atividade.ServicoId);
            atividade.EstimativaDias = servico?.EstimativaDias ?? 3;
            atividade.DataPrevista = CalcularDataPrevista((DateTime)atividade.DataInicio, atividade.EstimativaDias);

            // ✅ Garante a Etapa "Recebimento"
            if (atividade.EtapaId == 0 || !_context.Etapas.Any(e => e.Id == atividade.EtapaId))
            {
                var etapaRecebimento = _context.Etapas.FirstOrDefault(e => e.Nome.ToUpper() == "RECEBIMENTO");

                if (etapaRecebimento != null)
                {
                    atividade.EtapaId = etapaRecebimento.Id;
                }
                else
                {
                    ModelState.AddModelError("EtapaId", "A etapa 'Recebimento' não foi encontrada.");
                    PreencherDropdowns();
                    return View(atividade);
                }
            }
            _context.Atividades.Add(atividade);

            var salvo = await RetryHelper.TentarSalvarAsync(async () =>
            {
                await _context.SaveChangesAsync();
            });

            if (!salvo)
            {
                ModelState.AddModelError("", "Erro ao salvar: o banco estava ocupado. Tente novamente.");
                PreencherDropdowns();
                return View(atividade);
            }

            // ✅ Registrar histórico com retry também
            var historicoSalvo = await RetryHelper.TentarSalvarAsync(async () =>
            {
                RegistrarHistorico(atividade, "Criado");
                await _context.SaveChangesAsync();
            });

            if (!historicoSalvo)
            {
                TempData["Erro"] = "Atividade criada, mas falha ao registrar o histórico.";
                return RedirectToAction("Index");
            }

            TempData["Mensagem"] = $"Atividade foi criada com sucesso.";
            return RedirectToAction("Index");
        }


        private void PreencherDropdowns() {
            var funcionarios = _context.Funcionarios.ToList();
            var servicos = _context.Servicos.ToList();

            // Incluindo o nome do cliente junto com o modelo do carro
            var carros = _context.Carros
                .Include(c => c.Cliente)
                .Select(c => new
                {
                    c.Id,
                    ModeloCliente = c.Modelo.Nome + " - " + c.Cliente.Nome // Concatenando o modelo e o nome do cliente
                }).ToList();

            ViewBag.Funcionarios = new SelectList(funcionarios, "Id", "Nome");
            ViewBag.Servicos = new SelectList(servicos, "Id", "Descricao");

            // Passando a lista de carros com modelo e nome do cliente para a view
            ViewBag.Carros = new SelectList(carros, "Id", "ModeloCliente"); // ModeloCliente contém o modelo + cliente
        }

        private DateTime CalcularDataPrevista(DateTime dataInicio, int diasUteis) {
            var feriados = DataUtil.ObterFeriados(dataInicio.Year);

            DateTime data = dataInicio;
            int adicionados = 0;

            while (adicionados < diasUteis)
            {
                data = data.AddDays(1);
                if (data.DayOfWeek != DayOfWeek.Saturday &&
                    data.DayOfWeek != DayOfWeek.Sunday &&
                    !feriados.Contains(data.Date))
                {
                    adicionados++;
                }
            }

            return data;
        }

        [HttpGet]
        public IActionResult Edit(int id) {

            var atividade = _context.Atividades.Find(id);
            atividade.Cor = DefinirCorStatus(atividade.Status);
            ViewBag.Etapas = new SelectList(_context.Etapas.OrderBy(e => e.Ordem).ToList(), "Id", "Nome");

            if (atividade == null) return NotFound();

            PreencherDropdowns();
            return View(atividade);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Atividade atividade) {

            if (!atividade.DataInicio.HasValue)
                ModelState.AddModelError("DataInicio", "A data de início é obrigatória.");

            bool temErro = false;

            if (atividade.FuncionarioId == 0)
            {
                ModelState.AddModelError("FuncionarioId", "Selecione um funcionário.");
                temErro = true;
            }

            if (atividade.ServicoId == 0)
            {
                ModelState.AddModelError("ServicoId", "Selecione um serviço.");
                temErro = true;
            }


            if (!atividade.DataInicio.HasValue)
            {
                ModelState.AddModelError("DataInicio", "A data de início é obrigatória.");
                temErro = true;
            }

            if (temErro)
            {
                PreencherDropdowns(); // apenas chama
                return View(atividade);
            }

            var servico = _context.Servicos.FirstOrDefault(s => s.Id == atividade.ServicoId);
            if (servico == null)
            {
                ModelState.AddModelError("ServicoId", "Serviço inválido.");
                PreencherDropdowns();
                return View(atividade);
            }

            atividade.EstimativaDias = (int)servico.EstimativaDias;
            atividade.DataPrevista = CalcularDataPrevista((DateTime)atividade.DataInicio, (int)servico.EstimativaDias);
            atividade.Cor = DefinirCorStatus(atividade.Status);
            atividade.Etapa = _context.Etapas.FirstOrDefault(e => e.Id == atividade.EtapaId);

            _context.Atividades.Update(atividade);

            var salvo = await RetryHelper.TentarSalvarAsync(async () =>
            {
                await _context.SaveChangesAsync();
            });

            if (!salvo)
            {
                ModelState.AddModelError("", "Erro ao salvar alterações. O banco estava ocupado. Tente novamente.");
                PreencherDropdowns();
                return View(atividade);
            }

            var historicoSalvo = await RetryHelper.TentarSalvarAsync(async () =>
            {
                RegistrarHistorico(atividade, "Editado");
                await _context.SaveChangesAsync();
            });

            if (!historicoSalvo)
            {
                TempData["Erro"] = "Atividade editada, mas falha ao salvar o histórico.";
                return RedirectToAction("Index");
            }

            TempData["Mensagem"] = $"Atividade foi editada com sucesso.";
            return RedirectToAction("Index");
        }



        [HttpGet]
        public IActionResult Delete(int id) {
            var atividade = _context.Atividades
                .Include(a => a.Funcionario)
                .Include(a => a.Servico)
                .FirstOrDefault(a => a.Id == id);

            if (atividade == null) return NotFound();

            TempData["Mensagem"] = $"Atividade foi excluído com sucesso.";
            return View(atividade);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id) {

            var atividade = _context.Atividades.Find(id);
            if (atividade == null) return NotFound();

            _context.Atividades.Remove(atividade);

            _context.SaveChanges();

            var sucesso = await RetryHelper.TentarSalvarAsync(async () =>
            {
                await _context.SaveChangesAsync();
            });

            if (!sucesso)
            {
                TempData["Erro"] = "Erro ao excluir atividade. O banco estava ocupado.";
                return RedirectToAction("Index");
            }

            TempData["Mensagem"] = "Atividade foi excluída com sucesso.";
            return RedirectToAction("Index");
        }

        private async Task<bool> RegistrarHistorico(Atividade atividade, string acao) {
           
            // 🔒 Protege contra EtapaId inexistente ou nulo
            string etapaNome = "-";

            if (atividade.Etapa != null)
            {
                etapaNome = atividade.Etapa.Nome;
            }
            else if (atividade.EtapaId > 0)
            {
                etapaNome = _context.Etapas
                    .Where(e => e.Id == atividade.EtapaId)
                    .Select(e => e.Nome)
                    .FirstOrDefault() ?? "-";
            }

            var historico = new AtividadeHistorico
            {
                AtividadeId = atividade.Id,
                FuncionarioNome = atividade.Funcionario?.Nome
                    ?? _context.Funcionarios.Find(atividade.FuncionarioId)?.Nome
                    ?? "Desconhecido",

                ServicoDescricao = atividade.Servico?.Descricao
                    ?? _context.Servicos.Find(atividade.ServicoId)?.Descricao
                    ?? "Desconhecido",

                CarroId = atividade.Carro?.IdCarro
                    ?? _context.Carros.Find(atividade.CarroId)?.IdCarro
                    ?? "-",

                ModeloNome = atividade.Carro?.Modelo?.Nome
                    ?? _context.Carros.Include(c => c.Modelo)
                        .FirstOrDefault(c => c.Id == atividade.CarroId)?.Modelo?.Nome
                    ?? "-",

                Cliente = atividade.Carro?.Cliente?.Nome
                    ?? _context.Carros.Include(c => c.Cliente)
                        .FirstOrDefault(c => c.Id == atividade.CarroId)?.Cliente?.Nome
                    ?? "-",

                DataInicio = atividade.DataInicio,
                DataPrevista = atividade.DataPrevista,
                Status = atividade.Status ?? "-",
                EtapaAtual = etapaNome,
                DataRegistro = DateTime.Now,
                Acao = acao
            };

            _context.AtividadeHistoricos.Add(historico);
            return await RetryHelper.TentarSalvarAsync(async () =>
            {
                await _context.SaveChangesAsync();
            });
        }

    }
}
