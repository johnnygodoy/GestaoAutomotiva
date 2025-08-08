namespace GestaoAutomotiva.Models
{
    public class Cambio
    {
        public int Id { get; set; }
        public string Descricao { get; set; }

        public int? ModeloId { get; set; }
        public Modelo? Modelo { get; set; }
    }
}
