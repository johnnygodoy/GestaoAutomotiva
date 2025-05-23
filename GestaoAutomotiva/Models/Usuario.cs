﻿using System.ComponentModel.DataAnnotations;

namespace GestaoAutomotiva.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Email é obrigatório.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha é obrigatória.")]
        public string Senha { get; set; }

        [Required]
        public string Tipo { get; set; } // "Admin" ou "Usuario"
    }
}
