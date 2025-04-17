using System.ComponentModel.DataAnnotations;

namespace GestaoAutomotiva.Models
{
    public class Carro
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo Código do Carro é obrigatório.")]
        public string IdCarro { get; set; }

        [Required(ErrorMessage = "O campo Modelo é obrigatório.")]
        public string Modelo { get; set; }

        [Required(ErrorMessage = "O campo Cor é obrigatório.")]
        public string Cor { get; set; }

        public int ClienteId { get; set; }

        public Cliente Cliente { get; set; }
    }

}
