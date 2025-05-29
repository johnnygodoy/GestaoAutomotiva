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
    }
}
