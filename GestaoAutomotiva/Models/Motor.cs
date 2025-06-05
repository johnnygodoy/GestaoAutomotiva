using System.ComponentModel.DataAnnotations;

namespace GestaoAutomotiva.Models
{
    public class Motor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do motor é obrigatório.")]
        public string Nome { get; set; }

    
        public string? PlacaVeiculoDoador { get; set; }

        public string? NumeroMotor { get; set; }

        [Required(ErrorMessage = "Selecione o status do motor.")]
        public StatusMotor Status { get; set; }

        public string? Observacoes { get; set; }

        public int? ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        public int? ModeloId { get; set; }
        public Modelo? Modelo { get; set; }
    }

    public enum StatusMotor
    {
        Sem_Motor,
        Solicitado_Compra,
        Em_Estoque,
        Estoque_Revisado,
        Retifica,
        Montagem,
        Revisando,
        Teste_Motor
    }
}
