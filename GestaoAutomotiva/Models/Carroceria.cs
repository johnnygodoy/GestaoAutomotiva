namespace GestaoAutomotiva.Models
{
    public class Carroceria
    {
        public int Id { get; set; }
        public string Descricao { get; set; }

        public int? ModeloId { get; set; }
        public Modelo? Modelo { get; set; }
    }
}
