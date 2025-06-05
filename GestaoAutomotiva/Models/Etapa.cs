using System.ComponentModel.DataAnnotations;

namespace GestaoAutomotiva.Models
{
    public class Etapa
    {
        public int Id { get; set; }

      
        [Required(ErrorMessage = "O nome da Etapa é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nomeda Etapa deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }


        [Required(ErrorMessage = "O Número da Ordem é obrigatório.")]
        public int Ordem { get; set; } // Define a ordem visual na esteira

        public ICollection<Atividade> Atividades { get; set; }
    }    

}
