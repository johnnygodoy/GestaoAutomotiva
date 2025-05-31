using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class CadastroAcessoriosController : Controller
{
    private readonly AppDbContext _context;

    public CadastroAcessoriosController(AppDbContext context) {
        _context = context;
    }

    [HttpGet]
    public IActionResult Create() {
        ViewBag.Motores = _context.Motors?.OrderBy(m => m.Nome).ToList() ?? new List<Motor>();
        ViewBag.Cambios = _context.Cambios?.OrderBy(c => c.Descricao).ToList() ?? new List<Cambio>();
        ViewBag.Suspensoes = _context.Suspensaos?.OrderBy(s => s.Descricao).ToList() ?? new List<Suspensao>();
        ViewBag.RodasPneus = _context.RodasPneus?.OrderBy(r => r.Descricao).ToList() ?? new List<RodaPneu>();
        ViewBag.Carrocerias = _context.Carrocerias?.OrderBy(c => c.Descricao).ToList() ?? new List<Carroceria>();
        ViewBag.Capotas = _context.Capotas?.OrderBy(c => c.Descricao).ToList() ?? new List<Capota>();
        ViewBag.Modelos = _context.Modelos.OrderBy(m => m.Nome).ToList();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CriarModeloCarro(string nome) {
        if (!string.IsNullOrWhiteSpace(nome) && !_context.Modelos.Any(m => m.Nome.ToUpper() == nome.ToUpper()))
        {
            _context.Modelos.Add(new Modelo { Nome = nome.ToUpper() });
            _context.SaveChanges();
            TempData["Mensagem"] = "Modelo de carro cadastrado com sucesso!";
        }
        else
        {
            TempData["Mensagem"] = "Modelo já cadastrado ou inválido.";
        }
        return RedirectToAction("Create");
    }

    private bool AdicionarAcessorio<T>(DbSet<T> dbSet, Func<T, string> propSelector, Func<string, T> factory, string descricao) where T : class {
        if (!string.IsNullOrWhiteSpace(descricao) &&
            !dbSet.AsEnumerable().Any(e => (propSelector(e) ?? "").ToUpper() == descricao.ToUpper()))
        {
            dbSet.Add(factory(descricao.ToUpper()));
            _context.SaveChanges();
            return true;
        }
        return false;
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CriarMotor(string descricao) {
        TempData["Mensagem"] = AdicionarAcessorio(_context.Motors, m => m.Nome, nome => new Motor { Nome = nome }, descricao)
            ? "Motor cadastrado com sucesso!"
            : "Motor já cadastrado ou inválido.";
        return RedirectToAction("Create");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CriarCambio(string descricao) {
        TempData["Mensagem"] = AdicionarAcessorio(_context.Cambios, c => c.Descricao, desc => new Cambio { Descricao = desc }, descricao)
            ? "Câmbio cadastrado com sucesso!"
            : "Câmbio já cadastrado ou inválido.";
        return RedirectToAction("Create");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CriarSuspensao(string descricao, int modeloId) {

        TempData["Mensagem"] = AdicionarAcessorio(_context.Suspensaos, c => c.Descricao, desc => new Suspensao { Descricao = desc }, descricao)
          ? "Suspensão cadastrado com sucesso!"
          : "Suspensão já cadastrado ou inválido.";
        return RedirectToAction("Create");

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CriarRodasPneus(string descricao) {
        TempData["Mensagem"] = AdicionarAcessorio(_context.RodasPneus, r => r.Descricao, desc => new RodaPneu { Descricao = desc }, descricao)
            ? "Rodas/Pneus cadastrados com sucesso!"
            : "Rodas/Pneus já cadastrados ou inválidos.";
        return RedirectToAction("Create");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CriarCarroceria(string descricao) {
        TempData["Mensagem"] = AdicionarAcessorio(_context.Carrocerias, c => c.Descricao, desc => new Carroceria { Descricao = desc }, descricao)
            ? "Carroceria cadastrada com sucesso!"
            : "Carroceria já cadastrada ou inválida.";
        return RedirectToAction("Create");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CriarCapota(string descricao) {
        TempData["Mensagem"] = AdicionarAcessorio(_context.Capotas, c => c.Descricao, desc => new Capota { Descricao = desc }, descricao)
            ? "Capota cadastrada com sucesso!"
            : "Capota já cadastrada ou inválida.";
        return RedirectToAction("Create");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Excluir(string tipo, int id) {
        if (string.IsNullOrWhiteSpace(tipo))
        {
            TempData["Mensagem"] = "Tipo inválido.";
            return RedirectToAction("Create");
        }

        switch (tipo.ToLower())
        {
            case "modelo":
                var modelo = _context.Modelos.Find(id);
                if (modelo != null) _context.Modelos.Remove(modelo);
                break;
            case "motor":
                var motor = _context.Motors.Find(id);
                if (motor != null) _context.Motors.Remove(motor);
                break;
            case "cambio":
                var cambio = _context.Cambios.Find(id);
                if (cambio != null) _context.Cambios.Remove(cambio);
                break;
            case "suspensao":
                var suspensao = _context.Suspensaos.Find(id);
                if (suspensao != null) _context.Suspensaos.Remove(suspensao);
                break;
            case "rodaspneus":
                var rodas = _context.RodasPneus.Find(id);
                if (rodas != null) _context.RodasPneus.Remove(rodas);
                break;
            case "carroceria":
                var carroceria = _context.Carrocerias.Find(id);
                if (carroceria != null) _context.Carrocerias.Remove(carroceria);
                break;
            case "capota":
                var capota = _context.Capotas.Find(id);
                if (capota != null) _context.Capotas.Remove(capota);
                break;
        }

        _context.SaveChanges();
        TempData["Mensagem"] = $"{tipo.ToUpper()} removido com sucesso.";
        return RedirectToAction("Create");
    }
}
