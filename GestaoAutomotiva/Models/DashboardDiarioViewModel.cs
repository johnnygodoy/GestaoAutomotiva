namespace GestaoAutomotiva.Models
{
    public class DashboardDiarioViewModel
    {
        public DateTime Data { get; set; }
        public int Concluidas { get; set; }
        public int Andamento { get; set; }
        public int Canceladas { get; set; }
        public int Reprovadas { get; set; }
        public int Parados { get; set; }
    }
}
