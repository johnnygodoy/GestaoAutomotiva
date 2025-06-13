using System.ComponentModel.DataAnnotations;

namespace GestaoAutomotiva.Models
{
    public class Modelo
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        public ICollection<Motor> Motores { get; set; }
        public ICollection<Cambio> Cambios { get; set; }
        public ICollection<Suspensao> Suspensoes { get; set; }
        public ICollection<Carroceria> Carrocerias { get; set; }
        public ICollection<Capota> Capotas { get; set; }
        public ICollection<Escapamento> Escapamentos { get; set; }
        public ICollection<Painel> Paineis { get; set; }
    }
}
