using System.ComponentModel.DataAnnotations;

namespace GestaoAutomotiva.Models
{
    public class AcessoriosCarro
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo Modelo do Carro é obrigatório.")]
        public int ModeloId { get; set; }
     
        public Modelo Modelo { get; set; }

        [Required(ErrorMessage = "O campo Motor do Carro é obrigatório.")]
        public int MotorId { get; set; }

  
        public Motor Motor { get; set; }

        [Required(ErrorMessage = "O campo Câmbio do Carro é obrigatório.")]
        public int CambioId { get; set; }

        public Cambio Cambio { get; set; }

        [Required(ErrorMessage = "O campo suspensão do Carro é obrigatório.")]

        public int SuspensaoId { get; set; }

        public Suspensao Suspensao { get; set; }

        [Required(ErrorMessage = "O campo Rodae e Pneus do Carro é obrigatório.")]
        public int RodasPneusId { get; set; }

        public RodaPneu RodasPneus { get; set; }

        [Required(ErrorMessage = "O campo Carroceria do Carro é obrigatório.")]
        public int CarroceriaId { get; set; }
     
        public Carroceria Carroceria { get; set; }

        [Required(ErrorMessage = "O campo Capota do Carro é obrigatório.")]
        public int CapotaId { get; set; }
     
        public Capota Capota { get; set; }
    }


}
