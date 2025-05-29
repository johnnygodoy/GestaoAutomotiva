using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class CadastroAcessoriosController : Controller
{
    private readonly AppDbContext _context;

    public CadastroAcessoriosController(AppDbContext context) {
        _context = context;
    }

    [HttpGet]
    public IActionResult Create() {
        ViewBag.Motores = _context.Motors.OrderBy(m => m.Descricao).ToList();
        ViewBag.Cambios = _context.Cambios.OrderBy(c => c.Descricao).ToList();
        ViewBag.Suspensoes = _context.Suspensaos.OrderBy(s => s.Descricao).ToList();
        ViewBag.RodasPneus = _context.RodasPneus.OrderBy(r => r.Descricao).ToList();
        ViewBag.Carrocerias = _context.Carrocerias.OrderBy(c => c.Descricao).ToList();
        ViewBag.Capotas = _context.Capotas.OrderBy(c => c.Descricao).ToList();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CriarMotor(string descricao) {
        if (!string.IsNullOrWhiteSpace(descricao) && !_context.Motors.Any(m => m.Descricao.ToUpper() == descricao.ToUpper()))
        {
            _context.Motors.Add(new Motor { Descricao = descricao.ToUpper() });
            _context.SaveChanges();
            TempData["Mensagem"] = "Motor cadastrado com sucesso!";
        }
        else
        {
            TempData["Mensagem"] = "Motor já cadastrado ou inválido.";
        }
        return RedirectToAction("Create");
    }

    // Repete o padrão para os demais tipos:
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CriarCambio(string descricao) {
        if (!string.IsNullOrWhiteSpace(descricao) && !_context.Cambios.Any(c => c.Descricao.ToUpper() == descricao.ToUpper()))
        {
            _context.Cambios.Add(new Cambio { Descricao = descricao.ToUpper() });
            _context.SaveChanges();
            TempData["Mensagem"] = "Câmbio cadastrado com sucesso!";
        }
        else
        {
            TempData["Mensagem"] = "Câmbio já cadastrado ou inválido.";
        }
        return RedirectToAction("Create");
    }

    // Suspensão
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CriarSuspensao(string descricao) {
        if (!string.IsNullOrWhiteSpace(descricao) && !_context.Suspensaos.Any(s => s.Descricao.ToUpper() == descricao.ToUpper()))
        {
            _context.Suspensaos.Add(new Suspensao { Descricao = descricao.ToUpper() });
            _context.SaveChanges();
            TempData["Mensagem"] = "Suspensão cadastrada com sucesso!";
        }
        else
        {
            TempData["Mensagem"] = "Suspensão já cadastrada ou inválida.";
        }
        return RedirectToAction("Create");
    }

    // Rodas/Pneus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CriarRodasPneus(string descricao) {
        if (!string.IsNullOrWhiteSpace(descricao) && !_context.RodasPneus.Any(r => r.Descricao.ToUpper() == descricao.ToUpper()))
        {
            _context.RodasPneus.Add(new RodaPneu { Descricao = descricao.ToUpper() });
            _context.SaveChanges();
            TempData["Mensagem"] = "Rodas/Pneus cadastrados com sucesso!";
        }
        else
        {
            TempData["Mensagem"] = "Rodas/Pneus já cadastrados ou inválidos.";
        }
        return RedirectToAction("Create");
    }

    // Carroceria
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CriarCarroceria(string descricao) {
        if (!string.IsNullOrWhiteSpace(descricao) && !_context.Carrocerias.Any(c => c.Descricao.ToUpper() == descricao.ToUpper()))
        {
            _context.Carrocerias.Add(new Carroceria { Descricao = descricao.ToUpper() });
            _context.SaveChanges();
            TempData["Mensagem"] = "Carroceria cadastrada com sucesso!";
        }
        else
        {
            TempData["Mensagem"] = "Carroceria já cadastrada ou inválida.";
        }
        return RedirectToAction("Create");
    }

    // Capota
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CriarCapota(string descricao) {
        if (!string.IsNullOrWhiteSpace(descricao) && !_context.Capotas.Any(c => c.Descricao.ToUpper() == descricao.ToUpper()))
        {
            _context.Capotas.Add(new Capota { Descricao = descricao.ToUpper() });
            _context.SaveChanges();
            TempData["Mensagem"] = "Capota cadastrada com sucesso!";
        }
        else
        {
            TempData["Mensagem"] = "Capota já cadastrada ou inválida.";
        }
        return RedirectToAction("Create");
    }

    // Exclusão genérica (por tipo)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Excluir(string tipo, int id) {
        switch (tipo.ToLower())
        {
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

