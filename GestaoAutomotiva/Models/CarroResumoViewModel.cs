using System;

namespace GestaoAutomotiva.Models
{
    public class CarroResumoViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } // Ex: "Civic - João"
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public int Percentual { get; set; }
        public string Tooltip { get; set; }
        public string Cor { get; set; }    

        public List<AtividadeResumo> AtividadesResumo { get; set; } = new();
    }

    public class AtividadeResumo
    {
        public string Funcionario { get; set; }      
        public string Servico { get; set; }
        public int? DiasAtraso { get; set; }
        public string Status { get; set; }

        public string Etapa { get; set; }
    }
}
