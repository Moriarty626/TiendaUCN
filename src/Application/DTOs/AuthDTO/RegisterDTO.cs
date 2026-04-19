using System.ComponentModel.DataAnnotations;
using TiendaUCN.src.Application.Validators;

namespace TiendaUCN.src.Application.DTOs.AuthDTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MinLength(2, ErrorMessage = "El nombre debe tener al menos 2 caracteres.")]
        [MaxLength(20, ErrorMessage = "El nombre no puede exceder los 20 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s]+$", ErrorMessage = "El nombre solo puede contener letras.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no posee un formato válido.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "El RUT es obligatorio.")]
        // Se eliminó la 'F' que sobraba al final de la expresión regular
        [RegularExpression(@"^\d{7,8}-[0-9kK]$", ErrorMessage = "El RUT no posee el formato XXXXXXXX-X.")]
        [RutValidation(ErrorMessage = "El RUT ingresado no es válido.")]
        public required string Rut { get; set; }

        [Required(ErrorMessage = "El número de teléfono es obligatorio.")]
        [RegularExpression(@"^\+569\s\d{8}$", ErrorMessage = "El formato debe ser +569 XXXXXXXX.")]
        public required string PhoneNumber { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [BirthDateValidation]
        public required DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "El género es obligatorio.")]
        [RegularExpression(@"^(Masculino|Femenino|Otro)$", ErrorMessage = "El género debe ser Masculino, Femenino u Otro.")]
        public required string Gender { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*()_+\[\]{};':""\\|,.<>/?]).*$",
            ErrorMessage = "La contraseña debe contener al menos una mayúscula, un número y un carácter especial.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [MaxLength(20, ErrorMessage = "La contraseña debe tener como máximo 20 caracteres.")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "La confirmación de contraseña es obligatoria.")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public required string ConfirmPassword { get; set; }
    }
}