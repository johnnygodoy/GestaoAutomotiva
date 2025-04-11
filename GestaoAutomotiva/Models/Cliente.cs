using System.ComponentModel.DataAnnotations;

namespace GestaoAutomotiva.Models
{
    public class Cliente
    {
        public int Id { get; set; } // Identificador do Cliente

        [Required(ErrorMessage = "O nome do cliente é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do cliente deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; } // Nome do Cliente

        [Required(ErrorMessage = "O endereço do cliente é obrigatório.")]
        [StringLength(200, ErrorMessage = "O endereço do cliente deve ter no máximo 200 caracteres.")]
        public string Endereco { get; set; } // Endereço do Cliente

        // Validação de Telefone
        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [Phone(ErrorMessage = "O número de telefone não é válido.")]
        [RegularExpression(@"^\(\d{2}\)\s\d{4,5}-\d{4}$", ErrorMessage = "O telefone deve estar no formato (XX) XXXX-XXXX ou (XX) XXXXX-XXXX.")]
        public string Telefone { get; set; } // Telefone do Cliente

        // Validação de CPF
        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$", ErrorMessage = "O CPF deve estar no formato XXX.XXX.XXX-XX.")]
        public string CPF { get; set; } // CPF do Cliente

        // Relacionamento com Carros (Um cliente pode ter vários carros)
        public ICollection<Carro> Carros { get; set; }
    }
}
