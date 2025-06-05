// Models/ViewModels/EtapaViewModel.cs
namespace GestaoAutomotiva.Models.ViewModels
{
    public class EtapaViewModel
    {
        public Etapa Etapa { get; set; } = new Etapa();
        public List<Etapa> ListaEtapas { get; set; } = new();
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
    }
}
