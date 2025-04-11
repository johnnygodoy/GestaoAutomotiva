using System.ComponentModel.DataAnnotations;

namespace GestaoAutomotiva.Models
{
    public class Servico
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O tipo é obrigatório.")]
        public string Tipo { get; set; }


        [Required(ErrorMessage = "A estimativa é obrigatória.")]
        [Range(1, 365, ErrorMessage = "A estimativa deve ser entre 1 e 365 dias.")]
        public int? EstimativaDias { get; set; }
    }
}
