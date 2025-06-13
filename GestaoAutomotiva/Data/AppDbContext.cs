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
        public DbSet<OrdemServico> OrdemServicos { get; set; }
        public DbSet<AcessoriosCarro> AcessoriosCarros { get; set; }
        public DbSet<Motor> Motors { get; set; }
        public DbSet<Modelo> Modelos { get; set; }
        public DbSet<Cambio> Cambios { get; set; }
        public DbSet<Suspensao> Suspensaos { get; set; }
        public DbSet<Roda> Rodas { get; set; }
        public DbSet<Pneu> Pneus { get; set; }
        public DbSet<SantoAntonio> SantoAntonios { get; set; }
        public DbSet<Carroceria> Carrocerias { get; set; }
        public DbSet<Capota> Capotas { get; set; }
        public DbSet<Escapamento> Escapamentos { get; set; }
        public DbSet<Painel> Paineis { get; set; }
        public DbSet<AtividadeHistorico> AtividadeHistoricos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "gestaoAutomotiva.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Atividade>()
                .HasOne(a => a.Etapa)
                .WithMany()
                .HasForeignKey(a => a.EtapaId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<AcessoriosCarro>()
                 .HasOne(ac => ac.Modelo)
                 .WithMany()
                 .HasForeignKey(ac => ac.ModeloId)
                 .OnDelete(DeleteBehavior.Restrict); // para impedir exclusão se estiver vinculado


            modelBuilder.Entity<Carro>()
                .HasOne(c => c.Cliente)
                .WithMany(cl => cl.Carros)
                .HasForeignKey(c => c.ClienteId);

            modelBuilder.Entity<Carro>()
               .HasOne(c => c.Modelo)
               .WithMany()
               .HasForeignKey(c => c.ModeloId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrdemServico>()
                .Property(o => o.Tarefas).IsRequired(false);
            modelBuilder.Entity<OrdemServico>()
                .Property(o => o.Observacoes).IsRequired(false);
            modelBuilder.Entity<OrdemServico>()
                .Property(o => o.Almoxarifado).IsRequired(false);
            modelBuilder.Entity<OrdemServico>()
                .Property(o => o.Inspetor).IsRequired(false);

            modelBuilder.Entity<OrdemServico>()
                .HasOne(o => o.Atividade)
                .WithMany()
                .HasForeignKey(o => o.AtividadeId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Nome = "Administrador",
                    Email = "admin@gestao.com",
                    Senha = "admin123",
                    Tipo = "Admin"
                });

            modelBuilder.Entity<Modelo>().HasData(
                new Modelo { Id = 1, Nome = "FURLAN GT40" },
                new Modelo { Id = 2, Nome = "FURLAN COBRA" },
                new Modelo { Id = 3, Nome = "FURLAN SSK1929" }
            );

            modelBuilder.Entity<Motor>()
                .HasOne(m => m.Modelo)
                .WithMany(m => m.Motores)
                .HasForeignKey(m => m.ModeloId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Cambio>()
                .HasOne(c => c.Modelo)
                .WithMany(m => m.Cambios)
                .HasForeignKey(c => c.ModeloId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Suspensao>()
                .HasOne(s => s.Modelo)
                .WithMany(m => m.Suspensoes)
                .HasForeignKey(s => s.ModeloId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Carroceria>()
                .HasOne(c => c.Modelo)
                .WithMany(m => m.Carrocerias)
                .HasForeignKey(c => c.ModeloId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Capota>()
                .HasOne(c => c.Modelo)
                .WithMany(m => m.Capotas)
                .HasForeignKey(c => c.ModeloId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Escapamento>()
           .HasOne(c => c.Modelo)
           .WithMany(m => m.Escapamentos)
           .HasForeignKey(c => c.ModeloId)
           .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Painel>()
          .HasOne(c => c.Modelo)
          .WithMany(m => m.Paineis)
          .HasForeignKey(c => c.ModeloId)
          .OnDelete(DeleteBehavior.Restrict);
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
