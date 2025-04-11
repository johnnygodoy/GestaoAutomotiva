using System.ComponentModel.DataAnnotations;

namespace GestaoAutomotiva.Models
{
    public class Funcionario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório.")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "Especialidade é obrigatório.")]
        public string Especialidade { get; set; }

        [Required(ErrorMessage = "Status é obrigatório.")]
        public string Status { get; set; } = "Disponível"; // ou "Ocupado"
    }
}
