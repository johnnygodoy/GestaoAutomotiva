using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using GestaoAutomotiva.Utils;
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
    public async Task<IActionResult> CriarModeloCarro(string nome) {
        if (!string.IsNullOrWhiteSpace(nome) && !_context.Modelos.Any(m => m.Nome.ToUpper() == nome.ToUpper()))
        {
            _context.Modelos.Add(new Modelo { Nome = nome.ToUpper() });

            var sucesso = await RetryHelper.TentarSalvarAsync(async () =>
            {
                await _context.SaveChangesAsync();
            });

            TempData["Mensagem"] = sucesso
                ? "Modelo de carro cadastrado com sucesso!"
                : "Erro ao salvar o modelo. O banco estava ocupado. Tente novamente.";
        }
        else
        {
            TempData["Mensagem"] = "Modelo já cadastrado ou inválido.";
        }

        return RedirectToAction("Create");
    }

    private async Task<bool> AdicionarAcessorio<T>(DbSet<T> dbSet, Func<T, string> propSelector, Func<string, T> factory, string descricao) where T : class {
        if (!string.IsNullOrWhiteSpace(descricao) &&
            !dbSet.AsEnumerable().Any(e => (propSelector(e) ?? "").ToUpper() == descricao.ToUpper()))
        {
            dbSet.Add(factory(descricao.ToUpper()));
            var sucesso = await RetryHelper.TentarSalvarAsync(async () =>
            {
                await _context.SaveChangesAsync();
            });

            return sucesso;
        }

        return false;
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CriarMotor(string descricao) {
        TempData["Mensagem"] = await AdicionarAcessorio(_context.Motors, m => m.Nome, nome => new Motor { Nome = nome }, descricao)
            ? "Motor cadastrado com sucesso!"
            : "Motor já cadastrado ou inválido.";
        return RedirectToAction("Create");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CriarCambio(string descricao) {
        TempData["Mensagem"] = await AdicionarAcessorio(_context.Cambios, c => c.Descricao, desc => new Cambio { Descricao = desc }, descricao)
            ? "Câmbio cadastrado com sucesso!"
            : "Câmbio já cadastrado ou inválido.";
        return RedirectToAction("Create");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CriarSuspensao(string descricao, int modeloId) {

        TempData["Mensagem"] =await AdicionarAcessorio(_context.Suspensaos, c => c.Descricao, desc => new Suspensao { Descricao = desc }, descricao)
          ? "Suspensão cadastrado com sucesso!"
          : "Suspensão já cadastrado ou inválido.";
        return RedirectToAction("Create");

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CriarRodasPneus(string descricao) {
        TempData["Mensagem"] = await AdicionarAcessorio(_context.RodasPneus, r => r.Descricao, desc => new RodaPneu { Descricao = desc }, descricao)
            ? "Rodas/Pneus cadastrados com sucesso!"
            : "Rodas/Pneus já cadastrados ou inválidos.";
        return RedirectToAction("Create");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CriarCarroceria(string descricao) {
        TempData["Mensagem"] = await AdicionarAcessorio(_context.Carrocerias, c => c.Descricao, desc => new Carroceria { Descricao = desc }, descricao)
            ? "Carroceria cadastrada com sucesso!"
            : "Carroceria já cadastrada ou inválida.";
        return RedirectToAction("Create");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CriarCapota(string descricao) {
        TempData["Mensagem"] = await AdicionarAcessorio(_context.Capotas, c => c.Descricao, desc => new Capota { Descricao = desc }, descricao)
            ? "Capota cadastrada com sucesso!"
            : "Capota já cadastrada ou inválida.";
        return RedirectToAction("Create");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Excluir(string tipo, int id) {
        if (string.IsNullOrWhiteSpace(tipo))
        {
            TempData["Mensagem"] = "Tipo inválido.";
            return RedirectToAction("Create");
        }

        object entidade = tipo.ToLower() switch
        {
            "modelo" => _context.Modelos.Find(id),
            "motor" => _context.Motors.Find(id),
            "cambio" => _context.Cambios.Find(id),
            "suspensao" => _context.Suspensaos.Find(id),
            "rodaspneus" => _context.RodasPneus.Find(id),
            "carroceria" => _context.Carrocerias.Find(id),
            "capota" => _context.Capotas.Find(id),
            _ => null
        };

        if (entidade == null)
        {
            TempData["Mensagem"] = $"{tipo.ToUpper()} não encontrado.";
            return RedirectToAction("Create");
        }

        try
        {
            _context.Remove(entidade);
            await _context.SaveChangesAsync();
            TempData["Mensagem"] = $"{tipo.ToUpper()} removido com sucesso.";
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("FOREIGN KEY constraint failed") == true)
        {
            TempData["Mensagem"] = $"Não foi possível excluir. Esse {tipo.ToUpper()} está sendo utilizado.";
        }
        catch (Exception ex)
        {
            TempData["Mensagem"] = $"Erro ao excluir: {ex.Message}";
        }

        return RedirectToAction("Create");
    }

}
