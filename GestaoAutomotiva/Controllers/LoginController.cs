using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using GestaoAutomotiva.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;


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

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult Cadastrar(Usuario usuario) {
        if (!ModelState.IsValid)
            return View(usuario);

        if (_context.Usuarios.Any(u => u.Email == usuario.Email))
        {
            ModelState.AddModelError("Email", "Esse e-mail já está em uso.");
            return View(usuario);
        }

        _context.Usuarios.Add(usuario);
        _context.SaveChanges();

        TempData["Mensagem"] = "Usuário cadastrado com sucesso!";
        return RedirectToAction("Index", "Home");
    }
}
