using System.ComponentModel.DataAnnotations;

namespace GestaoAutomotiva.Models
{
    public class AcessoriosCarro
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo Modelo do Carro é obrigatório.")]
        public int ModeloId { get; set; }
     
        public Modelo Modelo { get; set; }
    
        public int? MotorId { get; set; }  
        public Motor Motor { get; set; }

        [Required(ErrorMessage = "O campo Câmbio do Carro é obrigatório.")]
        public int CambioId { get; set; }

        public Cambio Cambio { get; set; }

        [Required(ErrorMessage = "O campo suspensão do Carro é obrigatório.")]

        public int SuspensaoId { get; set; }

        public Suspensao Suspensao { get; set; }

        [Required(ErrorMessage = "O campo Rodas é obrigatório.")]
        public int RodaId { get; set; }
        public Roda Roda { get; set; }

        [Required(ErrorMessage = "O campo Pneus é obrigatório.")]
        public int PneuId { get; set; }
        public Pneu Pneu { get; set; }

        [Required(ErrorMessage = "O campo Santo Antônio é obrigatório.")]
        public int SantoAntonioId { get; set; }
        public SantoAntonio  SantoAntonio { get; set; }



        [Required(ErrorMessage = "O campo Carroceria do Carro é obrigatório.")]
        public int CarroceriaId { get; set; }
     
        public Carroceria Carroceria { get; set; }

        [Required(ErrorMessage = "O campo Capota do Carro é obrigatório.")]
        public int CapotaId { get; set; }
     
        public Capota Capota { get; set; }

        [Required(ErrorMessage = "O campo Escapamento do Carro é obrigatório.")]
        public int EscapamentoId { get; set; }
        public Escapamento Escapamento { get; set; }


        [Required(ErrorMessage = "O campo Painel do Carro é obrigatório.")]
        public int PainelId { get; set; }
        public Painel Painel { get; set; }
    }


}
