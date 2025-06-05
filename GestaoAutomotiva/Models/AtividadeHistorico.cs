using System;

namespace GestaoAutomotiva.Models
{
    public class AtividadeHistorico
    {
        public int Id { get; set; }

        public int? AtividadeId { get; set; } // Pode ser null se a atividade original for excluída

        public string FuncionarioNome { get; set; }

        public string ServicoDescricao { get; set; }

        public string CarroId { get; set; }

        public string ModeloNome { get; set; }

        public string Cliente { get; set; }

        public DateTime? DataInicio { get; set; }

        public DateTime? DataPrevista { get; set; }

        public string Status { get; set; }

        public string EtapaAtual { get; set; }

        public DateTime DataRegistro { get; set; }

        public string Acao { get; set; } // Ex: Criado, Editado, Finalizado
    }
}
