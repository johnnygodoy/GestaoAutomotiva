using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using GestaoAutomotiva.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;


public class LoginController : Controller
{
    private readonly AppDbContext _context;
    private readonly LicencaService _licencaService;

    public LoginController(AppDbContext context, LicencaService licencaService) {
        _context = context;
        _licencaService = licencaService;
    }

    public IActionResult Index() {
        if (_licencaService.LicencaExpirada)
            return Content("Licença expirada. Contate o suporte.");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(string email, string senha) {
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email && u.Senha == senha);
        if (usuario == null)
        {
            ViewBag.Erro = "Usuário ou senha inválidos.";
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Role, usuario.Tipo),
            new Claim("Email", usuario.Email)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout() {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index");
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Cadastrar() {
        return View();
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Excluir(int id) {
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == id);

        if (usuario == null)
        {
            TempData["Erro"] = "Usuário não encontrado.";
            return RedirectToAction("Index","Home"); 
        }

        return View(usuario);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ExcluirVarios(List<int> idsSelecionados) {
        if (idsSelecionados == null || !idsSelecionados.Any())
        {
            TempData["Erro"] = "Nenhum usuário foi selecionado.";
            return RedirectToAction("GerenciarUsuarios");
        }

        var usuarios = await _context.Usuarios
            .Where(u => idsSelecionados.Contains(u.Id))
            .ToListAsync();

        _context.Usuarios.RemoveRange(usuarios);

        bool salvo = await RetryHelper.TentarSalvarAsync(async () =>
        {
            await _context.SaveChangesAsync();
        });

        if (!salvo)
        {
            TempData["Erro"] = "Erro ao excluir usuários. O banco estava ocupado. Tente novamente.";
            return RedirectToAction("GerenciarUsuarios");
        }

        TempData["Mensagem"] = $"{usuarios.Count} usuário(s) excluído(s) com sucesso.";
        return RedirectToAction("GerenciarUsuarios");
    }




    [Authorize(Roles = "Admin")]
    public IActionResult GerenciarUsuarios(string busca = null) {
        var usuarios = _context.Usuarios.AsQueryable();

        if (!string.IsNullOrWhiteSpace(busca))
        {
            var buscaUpper = busca.Trim().ToUpper();
            usuarios = usuarios.Where(u =>
                u.Nome.ToUpper().Contains(buscaUpper) ||
                u.Email.ToUpper().Contains(buscaUpper));
        }

        return View(usuarios.ToList());
    }


    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Excluir(Usuario usuario) {
        var usuarioDb = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == usuario.Id);

        if (usuarioDb == null)
        {
            TempData["Erro"] = "Usuário não encontrado ou já foi excluído.";
            return RedirectToAction("Index", "Home");
        }

        _context.Usuarios.Remove(usuarioDb);

        bool salvo = await RetryHelper.TentarSalvarAsync(async () =>
        {
            await _context.SaveChangesAsync();
        });

        if (!salvo)
        {
            TempData["Erro"] = "Erro ao excluir o usuário. O banco estava ocupado. Tente novamente.";
            return RedirectToAction("Index", "Home");
        }

        TempData["Mensagem"] = $"Usuário {usuarioDb.Nome} foi excluído com sucesso.";
        return RedirectToAction("Index", "Home");
    }


    [Authorize]
    [HttpGet]
    public IActionResult AlterarSenha() {
        return View();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AlterarSenha(AlterarSenhaViewModel model) {
        if (!ModelState.IsValid)
            return View(model);

        var email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

        if (usuario == null || usuario.Senha != model.SenhaAtual)
        {
            TempData["Erro"] = "Senha atual incorreta.";
            return View(model);
        }

        usuario.Senha = model.NovaSenha;

        bool salvo = await RetryHelper.TentarSalvarAsync(async () =>
        {
            await _context.SaveChangesAsync();
        });

        if (!salvo)
        {
            TempData["Erro"] = "Erro ao alterar a senha. O banco estava ocupado. Tente novamente.";
            return View(model);
        }

        TempData["Mensagem"] = "Senha alterada com sucesso.";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Cadastrar(Usuario usuario) {
        if (!ModelState.IsValid)
            return View(usuario);

        bool emailEmUso = await _context.Usuarios.AnyAsync(u => u.Email == usuario.Email);
        if (emailEmUso)
        {
            ModelState.AddModelError("Email", "Esse e-mail já está em uso.");
            return View(usuario);
        }

        _context.Usuarios.Add(usuario);

        bool salvo = await RetryHelper.TentarSalvarAsync(async () =>
        {
            await _context.SaveChangesAsync();
        });

        if (!salvo)
        {
            TempData["Erro"] = "Erro ao cadastrar usuário. O banco estava ocupado. Tente novamente.";
            return View(usuario);
        }

        TempData["Mensagem"] = "Usuário cadastrado com sucesso!";
        return RedirectToAction("Index", "Home");
    }

}
