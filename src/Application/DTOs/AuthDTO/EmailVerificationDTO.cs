using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaUCN.src.Application.DTOs.AuthDTO
{
    public class EmailVerificationDTO
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "El código de verificación es obligatorio.")]
        public required string VerificationCode { get; set; }

    }
}