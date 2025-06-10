using System.ComponentModel.DataAnnotations;

namespace GestaoAutomotiva.Models
{
    public class AlterarSenhaViewModel
    {
        [Required(ErrorMessage = "Informe a senha atual.")]
        public string SenhaAtual { get; set; }

        [Required(ErrorMessage = "Informe a nova senha.")]
        [MinLength(6, ErrorMessage = "A nova senha deve ter pelo menos 6 caracteres.")]
        public string NovaSenha { get; set; }

        [Required(ErrorMessage = "Confirme a nova senha.")]
        [Compare("NovaSenha", ErrorMessage = "A nova senha e a confirmação não coincidem.")]
        public string ConfirmarSenha { get; set; }
    }
}
