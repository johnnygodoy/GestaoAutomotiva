using System.ComponentModel.DataAnnotations;

namespace GestaoAutomotiva.Models
{
    public class Etapa
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        public int Ordem { get; set; } // Define a ordem visual na esteira

        public ICollection<Atividade> Atividades { get; set; }
    }    

}
