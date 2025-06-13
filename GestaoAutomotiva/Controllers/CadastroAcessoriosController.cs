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
        ViewBag.Rodas = _context.Rodas?.OrderBy(r => r.Descricao).ToList() ?? new List<Roda>();
        ViewBag.Pneus = _context.Pneus?.OrderBy(r => r.Descricao).ToList() ?? new List<Pneu>();
        ViewBag.SantoAntonios = _context.SantoAntonios?.OrderBy(r => r.Descricao).ToList() ?? new List<SantoAntonio>();
        ViewBag.Carrocerias = _context.Carrocerias?.OrderBy(c => c.Descricao).ToList() ?? new List<Carroceria>();
        ViewBag.Capotas = _context.Capotas?.OrderBy(c => c.Descricao).ToList() ?? new List<Capota>();
        ViewBag.Escapamentos = _context.Escapamentos?.OrderBy(c => c.Descricao).ToList() ?? new List<Escapamento>();
        ViewBag.Paineis = _context.Paineis?.OrderBy(c => c.Descricao).ToList() ?? new List<Painel>();
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
    public async Task<IActionResult> CriarRoda(string descricao) {
        TempData["Mensagem"] = await AdicionarAcessorio(_context.Rodas, r => r.Descricao, desc => new Roda { Descricao = desc }, descricao)
            ? "Rodas cadastrados com sucesso!"
            : "Rodas já cadastrados ou inválidos.";
        return RedirectToAction("Create");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CriarPneu(string descricao) {
        TempData["Mensagem"] = await AdicionarAcessorio(_context.Pneus, r => r.Descricao, desc => new Pneu { Descricao = desc }, descricao)
            ? "Pneus cadastrados com sucesso!"
            : "Pneus já cadastrados ou inválidos.";
        return RedirectToAction("Create");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CriarSantoAntonio(string descricao) {
        TempData["Mensagem"] = await AdicionarAcessorio(_context.SantoAntonios, r => r.Descricao, desc => new SantoAntonio { Descricao = desc }, descricao)
            ? "Santo Antônio cadastrados com sucesso!"
            : "Santo Antônio já cadastrados ou inválidos.";
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
    public async Task<IActionResult> CriarEscapamento(string descricao) {
        TempData["Mensagem"] = await AdicionarAcessorio(_context.Escapamentos, c => c.Descricao, desc => new Escapamento { Descricao = desc }, descricao)
            ? "Escapamento cadastrada com sucesso!"
            : "Escapamento já cadastrada ou inválida.";
        return RedirectToAction("Create");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CriarPainel(string descricao) {
        TempData["Mensagem"] = await AdicionarAcessorio(_context.Paineis, c => c.Descricao, desc => new Painel { Descricao = desc }, descricao)
            ? "Painel cadastrada com sucesso!"
            : "Painel já cadastrada ou inválida.";
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
            "rodas" => _context.Rodas.Find(id),
            "pneus" => _context.Pneus.Find(id),
            "santoAntonio" => _context.SantoAntonios.Find(id),
            "carroceria" => _context.Carrocerias.Find(id),
            "capota" => _context.Capotas.Find(id),
            "escapamento" => _context.Escapamentos.Find(id),
            "paineis" => _context.Paineis.Find(id),
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
