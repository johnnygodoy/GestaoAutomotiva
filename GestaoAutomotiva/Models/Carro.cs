using System.ComponentModel.DataAnnotations;

namespace GestaoAutomotiva.Models
{
    public class Carro
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo Código do Carro é obrigatório.")]
        public string IdCarro { get; set; }

        [Display(Name = "Tipo de Manutenção")]
        [Required(ErrorMessage = "O campo Tipo de Manutenção é obrigatório.")]
        public string TipoManutencao { get; set; }

        [Required(ErrorMessage = "O campo Cor é obrigatório.")]
        public string Cor { get; set; }

        public int ClienteId { get; set; }

        public Cliente Cliente { get; set; }

        public int? AcessoriosCarroId { get; set; }  // FK
        public AcessoriosCarro Acessorios { get; set; }

        public int ModeloId { get; set; }
        public Modelo Modelo { get; set; }

    }

}
