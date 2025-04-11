using GestaoAutomotiva.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoAutomotiva.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Servico> Servicos { get; set; }
        public DbSet<Atividade> Atividades { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Etapa> Etapas { get; set; }
        public DbSet<Carro> Carros { get; set; }
        public DbSet<Cliente> Clientes { get; set; }

        // Método para configurar o caminho do banco de dados
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "gestaoAutomotiva.db");

            // Usando o SQLite com o caminho correto para o banco de dados
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Atividade>()
                .HasOne(a => a.Etapa)
                .WithMany() // Indica que uma etapa pode ter várias atividades
                .HasForeignKey(a => a.EtapaId) // Definindo a chave estrangeira
                .OnDelete(DeleteBehavior.SetNull); // Ao deletar uma etapa, a chave estrangeira é definida como null

            modelBuilder.Entity<Carro>()
                .HasOne(c => c.Cliente)
                .WithMany(cl => cl.Carros) // Um cliente pode ter vários carros
                .HasForeignKey(c => c.ClienteId);

            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Nome = "Administrador",
                    Email = "admin@gestao.com",
                    Senha = "admin123", // você pode trocar por uma senha melhor depois
                    Tipo = "Admin"
                }
            );
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
