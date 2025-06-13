using System.ComponentModel.DataAnnotations;

namespace GestaoAutomotiva.Models
{
    public class Roda
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Descrição da Roda é obrigatória.")]
        public string Descricao { get; set; }

        public int? ModeloId { get; set; }
        public Modelo? Modelo { get; set; }
    }

}
