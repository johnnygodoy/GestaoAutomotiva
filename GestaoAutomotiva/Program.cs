using GestaoAutomotiva.Data;
using GestaoAutomotiva.Models;
using GestaoAutomotiva.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

var envConn = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
string connStr;

if (!string.IsNullOrWhiteSpace(envConn))
{
    connStr = envConn; // pode ser Postgres (Host=...) ou SQLite (Data Source=...)
}
else
{
    var baseDir = Directory.Exists("/data") ? "/data" : AppContext.BaseDirectory;
    Directory.CreateDirectory(baseDir);
    connStr = $"Data Source={Path.Combine(baseDir, "gestaoAutomotiva.db")}";
}

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (connStr.Contains("Host=", StringComparison.OrdinalIgnoreCase))
        options.UseNpgsql(connStr);
    else
        options.UseSqlite(connStr);
});


builder.Services.AddControllersWithViews()
    .AddViewOptions(o => o.HtmlHelperOptions.ClientValidationEnabled = true);

builder.Services.AddSingleton<LicencaService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o => o.LoginPath = "/Login/Index");

var app = builder.Build();

// ===== Migrate + Seed =====
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    DbInitializer.SeedEtapas(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// IMPORTANTe p/ Render (respeitar X-Forwarded-Proto/For):
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

//Habilitar HTTPS redirection (Render já termina TLS no proxy):
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
