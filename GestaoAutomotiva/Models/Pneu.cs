using System.ComponentModel.DataAnnotations;

namespace GestaoAutomotiva.Models
{
    public class Pneu
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Descrição do Pneu é obrigatória.")]
        public string Descricao { get; set; }

        public int? ModeloId { get; set; }
        public Modelo? Modelo { get; set; }
    }

}
