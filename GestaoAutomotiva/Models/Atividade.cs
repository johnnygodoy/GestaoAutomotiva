using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoAutomotiva.Models
{
    public class Atividade
    {
        public int Id { get; set; }

        public int FuncionarioId { get; set; }
        public int ServicoId { get; set; }

        public DateTime? DataInicio { get; set; }
        public int EstimativaDias { get; set; }
        public DateTime? DataPrevista { get; set; }
        public string Status { get; set; } = "Em Andamento";

        public int? EtapaId { get; set; }  // Somente essa chave estrangeira
        public Etapa Etapa { get; set; }

        public Funcionario Funcionario { get; set; }
        public Servico Servico { get; set; }

        public int CarroId { get; set; }  // Relacionamento com Carro
        public Carro Carro { get; set; }  // Relacionamento com Carro

        [NotMapped]
        public bool Conflito { get; set; }

        public string Cor { get; set; } // Usada para colorir o gráfico Gantt

        [NotMapped]
        public bool Atrasado => DataPrevista.HasValue && DataPrevista.Value.Date < DateTime.Today && Status != "Finalizado";


    }

}
