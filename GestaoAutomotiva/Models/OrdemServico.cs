namespace GestaoAutomotiva.Models
{
    public class OrdemServico
    {
        public int Id { get; set; }
        public int? AtividadeId { get; set; }
        public Atividade? Atividade { get; set; }

        public string Prioridade { get; set; }
        public string EtapaAtual { get; set; }
        public DateTime DataAbertura { get; set; }
        public string Observacoes { get; set; }
        public int? FuncionarioId { get; set; }
        public int? CarroId { get; set; }
        public int? ClienteId { get; set; }

        public string Almoxarifado { get; set; }
        public string Inspetor { get; set; }
        public string Tarefas { get; set; }
    }


}
