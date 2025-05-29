using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
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
      .Include(c => c.Acessorios)
          .ThenInclude(a => a.Capota)
      .Include(c => c.Acessorios)
          .ThenInclude(a => a.Cambio)
      .Include(c => c.Acessorios)
          .ThenInclude(a => a.Carroceria)
      .Include(c => c.Acessorios)
          .ThenInclude(a => a.Motor)
      .Include(c => c.Acessorios)
          .ThenInclude(a => a.Suspensao)
      .Include(c => c.Acessorios)
          .ThenInclude(a => a.RodasPneus)
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
                Cliente = new Cliente(),
                Acessorios = new AcessoriosCarro()
            };

            // Carregar os modelos de carros, com valores temporários se não houver dados
            if (_context.Carros.Any())
            {
                ViewBag.Modelos = _context.Carros.Select(c => c.Modelo).Distinct().ToList();
            }
            else
            {
                ViewBag.Modelos = new List<string> { "FURLAN GT40", "FURLAN COBRA", "FURLAN DAYTONA", "FURLAN SSK1929" };
            }

            // Carregar acessórios: Se os dados existem, carregar do banco, caso contrário, valores temporários
            ViewBag.Motores = _context.Motors.Any()
         ? new SelectList(_context.Motors.ToList(), "Id", "Descricao")
         : new SelectList(new List<Motor>
         {
        new Motor { Id = 1, Descricao = "V4 200cc 2.0" },
        new Motor { Id = 2, Descricao = "V6 220cc 2.0" },
        new Motor { Id = 3, Descricao = "V8 280cc 4.7" },
        new Motor { Id = 4, Descricao = "V8 400cc 4.4" },
        new Motor { Id = 5, Descricao = "V8 550cc 5.0" },
         }, "Id", "Descricao");


            ViewBag.Cambios = _context.Cambios.Any()
                ? new SelectList(_context.Cambios.ToList(), "Id", "Descricao")
                : new SelectList(new List<Cambio>
                {
                    new Cambio {Id =1, Descricao ="Cambio Manual" },
                     new Cambio {Id =2, Descricao ="Cambio Automático" },
                }, "Id", "Descricao");


            ViewBag.Suspensoes = _context.Suspensaos.Any()
                ? new SelectList(_context.Suspensaos.ToList(), "Id", "Descricao")
                : new SelectList(new List<Suspensao>
                {
                    new Suspensao{ Id=1,Descricao = "Suspensão Rua" },
                    new Suspensao{ Id=2,Descricao = "Suspensão Pista" },

                }, "Id", "Descricao");



            ViewBag.RodasPneus = _context.RodasPneus.Any()
                ? new SelectList(_context.RodasPneus.ToList(), "Id", "Descricao")
                : new SelectList(new List<RodaPneu>
                {new  RodaPneu{ Id =1, Descricao = "ARO 17 Pneu Nacional" },

                new RodaPneu{Id=2, Descricao =  "ARO 15 Pneu Importado"},

                }, "Id", "Descricao");



            ViewBag.Carrocerias = _context.Carrocerias.Any()
                ? new SelectList(_context.Carrocerias.ToList(), "Id", "Descricao")
                : new SelectList(new List<Carroceria>

                {
                    new Carroceria { Id= 1, Descricao =  "Carroceria de Fibra de Carbono"},
                    new Carroceria { Id= 2, Descricao =  "Carroceria de Fibra de Vidro"},
                    new Carroceria { Id= 3, Descricao =  "Carroceria Hibrida"},
            }, "Id", "Descricao");


            ViewBag.Capotas = _context.Capotas.Any()
              ? new SelectList(_context.Capotas.ToList(), "Id", "Descricao")
              : new SelectList(new List<Capota>

              {
                    new Capota { Id= 1, Descricao =  "Escamoteável"},
                    new Capota { Id= 2, Descricao =  "Hard Top"},
                    new Capota { Id= 3, Descricao =  "Marítima"},
          }, "Id", "Descricao");


            return View(carro);
        }





        // Método POST para Criar Carro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Carro carro) {
            bool temErro = false;

            // Validações básicas (poderiam ser mantidas com ModelState.IsValid, mas vamos com as manuais que você já usa)
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

            if (string.IsNullOrEmpty(carro.Cor))
            {
                ModelState.AddModelError("Cor", "Campo Cor é obrigatório.");
                temErro = true;
            }

            // Cliente
            if (carro.Cliente == null ||
                string.IsNullOrWhiteSpace(carro.Cliente.Nome) ||
                string.IsNullOrWhiteSpace(carro.Cliente.Endereco) ||
                string.IsNullOrWhiteSpace(carro.Cliente.Telefone) ||
                string.IsNullOrWhiteSpace(carro.Cliente.CPF))
            {
                ModelState.AddModelError("Cliente", "Todos os campos do cliente são obrigatórios.");
                temErro = true;
            }

            // Acessórios
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
                return View(carro);
            }

            // 1️⃣ Salva o cliente primeiro
            _context.Clientes.Add(carro.Cliente);
            _context.SaveChanges();
            carro.ClienteId = carro.Cliente.Id;

            // 2️⃣ Cria e salva os acessórios (apenas com os IDs)
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

            // 3️⃣ Desanexa objetos para evitar conflito de reinsert
            carro.Cliente = null;
            carro.Acessorios = null;

            // 4️⃣ Salva o carro
            _context.Carros.Add(carro);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }




        // Tela de Editar Carro
        public IActionResult Edit(int id) {
            var carro = _context.Carros.Include(c => c.Cliente).FirstOrDefault(c => c.Id == id);
            carro = _context.Carros.Include(a => a.Acessorios).FirstOrDefault(a => a.Id == id);

            if (carro == null)
                return NotFound();

            // Garantir que os dados não sejam null antes de passá-los para a view
            ViewBag.Carros = _context.Carros.ToList() ?? new List<Carro>();
            ViewBag.Clientes = _context.Clientes.ToList() ?? new List<Cliente>();


            // Carregar os modelos de carros, com valores temporários se não houver dados
            var modelos = _context.Carros
            .Select(c => c.Modelo)
            .Distinct()
            .ToList();

            ViewBag.Modelos = modelos.Any()
                ? modelos
                : new List<string> { "FURLAN GT40", "FURLAN COBRA", "FURLAN DAYTONA", "FURLAN SSK1929" };

            // Carregar acessórios: Se os dados existem, carregar do banco, caso contrário, valores temporários

            ViewBag.Motores = new SelectList(
                 _context.Motors.ToList(), // Todos os motores disponíveis
                 "Id",
                 "Descricao",
                 carro.Acessorios?.MotorId // Motor salvo no carro atual
            );

            ViewBag.Cambios = new SelectList(
                _context.Cambios.ToList(),
                "Id",
                "Descricao",
                carro.Acessorios?.CambioId
             );

            ViewBag.Suspensoes = new SelectList(
                _context.Suspensaos.ToList(),
                "Id",
                "Descricao",
                carro.Acessorios?.SuspensaoId
                );

            ViewBag.RodasPneus = new SelectList(

                _context.RodasPneus.ToList(),
                "Id",
                "Descricao",
                carro.Acessorios.RodasPneusId
                );

            ViewBag.Carrocerias = new SelectList(
            _context.Carrocerias.ToList(),
            "Id",
            "Descricao",
            carro.Acessorios.CarroceriaId
            );

            ViewBag.Capotas = new SelectList(
            _context.Capotas.ToList(),
            "Id",
            "Descricao",
            carro.Acessorios.CapotaId
            );

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

                if (carro.Acessorios.MotorId == null)
                {
                    ModelState.AddModelError("carro.Acessorios.Motor", "Motor é obrigatório.");
                    temErro = true;
                }

                if (carro.Acessorios.CambioId == null)
                {
                    ModelState.AddModelError("carro.Acessorios.Motor", "Câmbio é obrigatório.");
                    temErro = true;
                }

                if (carro.Acessorios.SuspensaoId == null)
                {
                    ModelState.AddModelError("carro.Acessorios.SuspensaoId", "Suspensão é obrigatório.");
                    temErro = true;
                }
            }

            if (temErro)
            {
                ViewBag.Carros = _context.Carros.ToList();
                return View(carro);
            }

            // Busca entidades existentes
            var carroExistente = _context.Carros.Find(carro.Id);
            var clienteExistente = _context.Clientes.Find(carro.ClienteId);
            var acessoriosExistente = _context.AcessoriosCarros.Find(carro.AcessoriosCarroId);
            if (clienteExistente != null)
            {
                clienteExistente.Nome = carro.Cliente.Nome.ToUpper();
                clienteExistente.Endereco = carro.Cliente.Endereco.ToUpper();
                clienteExistente.Telefone = carro.Cliente.Telefone;
                clienteExistente.CPF = carro.Cliente.CPF;
                _context.Clientes.Update(clienteExistente);
            }
            else
            {
                ModelState.AddModelError("ClienteId", "Cliente não encontrado.");
                return View(carro);
            }

            // Atualiza os acessórios

            if (acessoriosExistente != null)
            {
                acessoriosExistente.MotorId = carro.Acessorios.MotorId;
                acessoriosExistente.CambioId = carro.Acessorios.CambioId;
                acessoriosExistente.SuspensaoId = carro.Acessorios.SuspensaoId;
                acessoriosExistente.CarroceriaId = carro.Acessorios.CarroceriaId;
                acessoriosExistente.RodasPneusId = carro.Acessorios.RodasPneusId;
                _context.AcessoriosCarros.Update(acessoriosExistente);
            }
            else
            {
                ModelState.AddModelError("AcessoriosCarroId", "Acessório não encontrado.");
                return View(carro);
            }

            // Atualiza Carro
            carroExistente.IdCarro = carro.IdCarro;
            carroExistente.Modelo = carro.Modelo?.ToUpper();
            carroExistente.Cor = carro.Cor?.ToUpper();
            carroExistente.ClienteId = carro.ClienteId;
            carroExistente.AcessoriosCarroId = carro.AcessoriosCarroId;

            _context.Carros.Update(carroExistente);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                // Aqui você pode logar o erro ou mostrar mensagem ao usuário
                ModelState.AddModelError("", "Erro ao salvar as alterações. Verifique os dados.");
                return View(carro);
            }

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
