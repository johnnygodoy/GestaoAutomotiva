using GestaoAutomotiva.Data;

namespace GestaoAutomotiva.Models
{
    public class DbInitializer
    {
        public static void SeedEtapas(AppDbContext context) {
            if (!context.Etapas.Any())
            {
                context.Etapas.AddRange(
                    new Etapa { Nome = "Recebimento", Ordem = 1 },
                    new Etapa { Nome = "Funilaria", Ordem = 2 },
                    new Etapa { Nome = "Pintura", Ordem = 3 },
                    new Etapa { Nome = "Montagem", Ordem = 4 },
                    new Etapa { Nome = "Finalizado", Ordem = 5 }
                );
                context.SaveChanges();
            }
        }
    }
}
