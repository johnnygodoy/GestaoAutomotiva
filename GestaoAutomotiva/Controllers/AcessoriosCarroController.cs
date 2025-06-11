using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using GestaoAutomotiva.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GestaoAutomotiva.Controllers
{
    public class AcessoriosCarroController : Controller
    {
        private readonly AppDbContext _context;

        public AcessoriosCarroController(AppDbContext context) {
            _context = context;
        }

        // Método para carregar todos os drop-downs
        private void CarregarDropdowns() {
            ViewBag.Motores = new SelectList(_context.Motors.OrderBy(m => m.Nome), "Id", "Nome");
            ViewBag.Cambios = new SelectList(_context.Cambios.OrderBy(c => c.Descricao), "Id", "Descricao");
            ViewBag.Suspensoes = new SelectList(_context.Suspensaos.OrderBy(s => s.Descricao), "Id", "Descricao");
            ViewBag.RodasPneus = new SelectList(_context.RodasPneus.OrderBy(r => r.Descricao), "Id", "Descricao");
            ViewBag.Carrocerias = new SelectList(_context.Carrocerias.OrderBy(c => c.Descricao), "Id", "Descricao");
            ViewBag.Capotas = new SelectList(_context.Capotas.OrderBy(c => c.Descricao), "Id", "Descricao");
        }

        private async Task PopularDadosIniciais() {
            _context.Motors.AddRange(new[]
            {
            new Motor { 
                Nome = "V4 200cc 2.0", 
                PlacaVeiculoDoador = "",
                NumeroMotor = "",
                Status = StatusMotor.Sem_Motor,
                Observacoes = "" },

    });

            _context.Cambios.AddRange(new[]
            {
        new Cambio { Descricao = "Cambio Manual" },
        new Cambio { Descricao = "Cambio Automático" },
    });

            _context.Suspensaos.AddRange(new[]
            {
        new Suspensao { Descricao = "Suspensão Rua" },
        new Suspensao { Descricao = "Suspensão Pista" },
    });

            _context.RodasPneus.AddRange(new[]
            {
        new RodaPneu { Descricao = "ARO 17 Pneu Nacional" },
        new RodaPneu { Descricao = "ARO 15 Pneu Importado" },
    });

            _context.Carrocerias.AddRange(new[]
            {
        new Carroceria { Descricao = "Carroceria de Fibra de Carbono" },
        new Carroceria { Descricao = "Carroceria de Fibra de Vidro" },
        new Carroceria { Descricao = "Carroceria Hibrida" },
    });

            _context.Capotas.AddRange(new[]
            {
        new Capota { Descricao = "Escamoteável" },
        new Capota { Descricao = "Hard Top" },
        new Capota { Descricao = "Marítima" },
    });

            var sucesso = await RetryHelper.TentarSalvarAsync(async () =>
            {
                await _context.SaveChangesAsync();
            });

            if (!sucesso)
            {
                // log ou tratamento opcional
                throw new Exception("Erro ao salvar dados iniciais: o banco de dados estava ocupado.");
            }
        }


        // GET: /AcessoriosCarro/Create
        public IActionResult Create() {

            if (!_context.Motors.Any())
            {
                if (!_context.Motors.Any())
                {
                    PopularDadosIniciais().GetAwaiter().GetResult(); // ← executa o async de forma síncrona
                }
            }
            CarregarDropdowns();
            return View();
        }

        // POST: /AcessoriosCarro/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AcessoriosCarro acessorios) {


            if (ModelState.IsValid)
            {

                CarregarDropdowns();
                return View(acessorios);
            }

            // Verificar duplicidade
            var duplicado = _context.AcessoriosCarros.Any(a =>
                a.MotorId == acessorios.MotorId &&
                a.CambioId == acessorios.CambioId &&
                a.SuspensaoId == acessorios.SuspensaoId &&
                a.RodasPneusId == acessorios.RodasPneusId &&
                a.CarroceriaId == acessorios.CarroceriaId &&
                a.CapotaId == acessorios.CapotaId
            );

            if (duplicado)
            {
                ModelState.AddModelError("", "Já existe um conjunto de acessórios com essas configurações.");
                CarregarDropdowns();
                return View(acessorios);
            }

            var sucesso = await RetryHelper.TentarSalvarAsync(async () =>
            {
                await _context.SaveChangesAsync();
            });

            if (!sucesso)
            {
                ModelState.AddModelError("", "Erro ao salvar: o banco de dados estava ocupado. Tente novamente.");
                CarregarDropdowns();
                return View(acessorios);
            }

            TempData["Mensagem"] = "Acessório criado com sucesso!";
            TempData["MaisUm"] = true;

            return RedirectToAction("Create");
        }
    }
}
