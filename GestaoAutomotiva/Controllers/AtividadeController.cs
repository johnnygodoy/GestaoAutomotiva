using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using GestaoAutomotiva.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public IActionResult Finalizar(int id) {
            var atividade = _context.Atividades.Find(id);
            if (atividade == null) return NotFound();

            atividade.Status = "Finalizado";
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        public IActionResult Index(string busca = null, string dataBusca = null,int page = 1) {
            ViewData["Busca"] = busca;
            ViewData["DataBusca"] = dataBusca;

            int pageSize = 10; // Quantidade de itens por página

            // Inicializando a query de atividades
            var atividades = _context.Atividades
                .Include(a => a.Funcionario)
                .Include(a => a.Servico)
                .Include(a => a.Carro) // Incluindo o carro para usar seus dados na pesquisa
                .ThenInclude(c => c.Cliente) // Incluindo o cliente relacionado ao carro
                .AsQueryable();

            // Busca por texto (funcionário, carro, modelo, serviço, status, etc.)
            if (!string.IsNullOrWhiteSpace(busca))
            {
                var buscaUpper = busca.ToUpper(); // Converte a busca para maiúsculas

                atividades = atividades.Where(a =>
                    a.Carro.Cliente.Nome.ToUpper().Contains(buscaUpper) ||  // Cliente
                    a.Carro.Modelo.ToUpper().Contains(buscaUpper) ||        // Modelo do carro
                    a.Funcionario.Nome.ToUpper().Contains(buscaUpper) ||    // Nome do funcionário
                    a.Servico.Descricao.ToUpper().Contains(buscaUpper) ||   // Descrição do serviço
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
        public IActionResult Create(Atividade atividade) {
            var funcionarios = _context.Funcionarios.ToList();
            var servicos = _context.Servicos.ToList();
            
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

            var carro = _context.Carros.Find(atividade.CarroId);

            if (carro != null)
            {
                atividade.Carro = carro;
                atividade.Carro.Modelo = atividade.Carro.Modelo.ToUpper();               
            }

            if (string.IsNullOrEmpty(atividade.Carro.Modelo))
            {
                ModelState.AddModelError("Carro", "O campo Carro é obrigatório.");
                temErro = true;
            }

            if (string.IsNullOrWhiteSpace(atividade.Carro.Cor))
            {
                ModelState.AddModelError("Cor", "O campo Cor é obrigatório.");
                temErro = true;
            }
            else
            {
                atividade.Carro.Cor = atividade.Carro.Cor.ToUpper();
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

            var servico = servicos.FirstOrDefault(s => s.Id == atividade.ServicoId);
            if (servico == null)
            {
                ModelState.AddModelError("ServicoId", "Serviço não encontrado no banco.");
                ViewBag.Funcionarios = new SelectList(funcionarios, "Id", "Nome");
                ViewBag.Servicos = new SelectList(servicos, "Id", "Descricao");
                return View(atividade);
            }

            atividade.EstimativaDias = (int)servico.EstimativaDias;
            atividade.DataPrevista = CalcularDataPrevista((DateTime)atividade.DataInicio, (int)servico.EstimativaDias);
            atividade.Status = "Em Andamento";

            // Verifica se a atividade é a primeira a ser criada
            if (atividade.EtapaId == 0 || atividade.EtapaId == null) // caso não tenha etapa associada
            {
                // Define a etapa "Recebimento" como a etapa inicial da atividade
                var etapaInicial = _context.Etapas.FirstOrDefault(e => e.Nome == "Recebimento");
                atividade.EtapaId = etapaInicial?.Id ?? 1; // Defina como etapa 1 se não encontrar a etapa de recebimento
            }

            _context.Atividades.Add(atividade);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        private void PreencherDropdowns() {
            var funcionarios = _context.Funcionarios.ToList();
            var servicos = _context.Servicos.ToList();

            // Incluindo o nome do cliente junto com o modelo do carro
            var carros = _context.Carros
                .Include(c => c.Cliente)
                .Select(c => new {
                    c.Id,
                    ModeloCliente = c.Modelo + " - " + c.Cliente.Nome // Concatenando o modelo e o nome do cliente
                }).ToList();

            ViewBag.Funcionarios = new SelectList(funcionarios, "Id", "Nome");
            ViewBag.Servicos = new SelectList(servicos, "Id", "Descricao");

            // Passando a lista de carros com modelo e nome do cliente para a view
            ViewBag.Carros = new SelectList(carros, "Id", "ModeloCliente"); // ModeloCliente contém o modelo + cliente
        }


        private DateTime CalcularDataPrevista(DateTime dataInicio, int diasUteis) {
            var feriados = new List<DateTime>
            {
                new DateTime(dataInicio.Year, 1, 1),   // Ano Novo
                new DateTime(dataInicio.Year, 12, 25)  // Natal
                // Pode adicionar mais aqui ou carregar de banco no futuro
            };

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

            if (atividade == null) return NotFound();

            PreencherDropdowns();
            return View(atividade);
        }

        [HttpPost]
        public IActionResult Edit(Atividade atividade) {                    

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

            _context.Atividades.Update(atividade);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id) {
            var atividade = _context.Atividades
                .Include(a => a.Funcionario)
                .Include(a => a.Servico)
                .FirstOrDefault(a => a.Id == id);

            if (atividade == null) return NotFound();
            return View(atividade);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id) {
            var atividade = _context.Atividades.Find(id);
            if (atividade == null) return NotFound();

            _context.Atividades.Remove(atividade);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Historico(string busca, string dataBusca = null, int page = 1 ) {
            ViewData["Busca"] = busca;
            ViewData["DataBusca"] = dataBusca;

            int pageSize = 10; // Quantidade de itens por página

            var atividades = _context.Atividades
            .Include(a => a.Funcionario)
            .Include(a => a.Servico)
            .Include(a => a.Carro)
            .ThenInclude(c => c.Cliente)
            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                var buscaUpper = busca.ToUpper(); // Converte a busca para maiúsculas

                atividades = atividades.Where(a =>
                    a.Carro.Modelo.ToUpper().Contains(buscaUpper) ||
                    a.Carro.IdCarro.Contains(buscaUpper) ||
                    a.Funcionario.Nome.ToUpper().Contains(buscaUpper) ||
                    a.Servico.Descricao.ToUpper().Contains(buscaUpper) ||
                    a.Status.ToUpper().Contains(buscaUpper) ||
                    a.Carro.Cliente.Nome.ToUpper().Contains(buscaUpper)
                    );
            }
            // Busca por data
            if (!string.IsNullOrWhiteSpace(dataBusca) && DateTime.TryParse(dataBusca, out DateTime dataBuscaConvertida))
            {
                atividades = atividades.Where(a =>
                    a.DataInicio == dataBuscaConvertida.Date ||
                    a.DataPrevista == dataBuscaConvertida.Date);
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

            return View(atividadesPaginados);
        }
        public IActionResult ExportarPdf() {
            var atividades = _context.Atividades
                .Include(a => a.Funcionario)
                .Include(a => a.Servico)
                .Where(a => a.Status != "Em Andamento")
                .ToList();

            var documento = new RelatorioAtividadePdf(atividades);
            var pdfBytes = documento.GeneratePdf(); // Isso retorna byte[]

            return File(pdfBytes, "application/pdf", "historico_atividades.pdf");

        }
        public IActionResult ExportarExcel() {
            var atividades = _context.Atividades
                .Include(a => a.Funcionario)
                .Include(a => a.Servico)
                .Where(a => a.Status != "Em Andamento")
                .ToList();

            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Histórico");

            worksheet.Cell(1, 1).Value = "Funcionário";
            worksheet.Cell(1, 2).Value = "Serviço";
            worksheet.Cell(1, 3).Value = "Código Carro";
            worksheet.Cell(1, 4).Value = "Carro";
            worksheet.Cell(1, 5).Value = "Início";
            worksheet.Cell(1, 6).Value = "Previsão";
            worksheet.Cell(1, 7).Value = "Status";

            for (int i = 0; i < atividades.Count; i++)
            {
                var a = atividades[i];
                worksheet.Cell(i + 2, 1).Value = a.Funcionario?.Nome;
                worksheet.Cell(i + 2, 2).Value = a.Servico?.Descricao;
                worksheet.Cell(i + 2, 3).Value = a.Carro.IdCarro;
                worksheet.Cell(i + 2, 4).Value = a.Carro.Modelo;
                worksheet.Cell(i + 2, 5).Value = a.DataInicio?.ToString("dd/MM/yyyy");
                worksheet.Cell(i + 2, 6).Value = a.DataPrevista?.ToString("dd/MM/yyyy");
                worksheet.Cell(i + 2, 7).Value = a.Status;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "historico_atividades.xlsx");
        }

    }
}
