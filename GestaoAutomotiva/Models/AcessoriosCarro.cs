namespace GestaoAutomotiva.Models
{
    public class AcessoriosCarro
    {
        public int Id { get; set; }

        public int MotorId { get; set; }
        public Motor Motor { get; set; }

        public int CambioId { get; set; }
        public Cambio Cambio { get; set; }

        public int SuspensaoId { get; set; }
        public Suspensao Suspensao { get; set; }

        public int RodasPneusId { get; set; }
        public RodaPneu RodasPneus { get; set; }

        public int CarroceriaId { get; set; }
        public Carroceria Carroceria { get; set; }

        public int CapotaId { get; set; }
        public Capota Capota { get; set; }
    }


}
