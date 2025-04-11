namespace GestaoAutomotiva.Models
{
    public class Carro
    {
        public int Id { get; set; }

        public string IdCarro { get; set; } // Código do Carro
        public string Modelo { get; set; } // Modelo do Carro

        public string Cor { get; set; } // Cor do Carro       

        // Relacionamento com Cliente
        public int ClienteId { get; set; } // Chave estrangeira para Cliente
        public Cliente Cliente { get; set; } // Navegação para o Cliente
    }
}
