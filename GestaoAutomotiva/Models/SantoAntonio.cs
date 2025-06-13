using System.ComponentModel.DataAnnotations;

namespace GestaoAutomotiva.Models
{
    public class SantoAntonio
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Descrição do Santo Antonio é obrigatória.")]
        public string Descricao { get; set; }

        public int? ModeloId { get; set; }
        public Modelo? Modelo { get; set; }
    }
}
