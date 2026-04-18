using System.ComponentModel.DataAnnotations;
using TiendaUCN.src.Application.Validators;


namespace TiendaUCN.src.Application.DTOs.AuthDTO

{
    public class RegisterDTO
    {
        //Validaciones de los campos del registro
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MinLength(2, ErrorMessage = "El nombre debe tener al menos 2 caracteres.")]
        [MaxLength(20, ErrorMessage = "El nombre no puede exceder los 20 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s]+$", ErrorMessage = "El nombre solo puede contener caracteres del abecedario español.")]

        public required string Name { get; set; }
        [Required(ErrorMessage = "El correo electronico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no posee el fomato valido.")]

        public required string Email { get; set; }
        [Required(ErrorMessage = "El Rut es obligatorio.")]
        [RegularExpression(@"^\d{7,8}-[0-9kK]F$", ErrorMessage = "El Rut no posee el formato XXXXXXXX-X.")]
        [RutValidation(ErrorMessage = "El Rut no es válido.")]
        public required string Rut { get; set; }
        [Required(ErrorMessage = "El numero de telefono es obligatorio.")]
        [RegularExpression(@"^\+569\s\d{8}$", ErrorMessage = "El numero de telefono debe contener 9 digitos y seguir el siguiente fomato +569 XXXXXXXX.")]

        public required string PhoneNumber { get; set; }
        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [BirthDateValidation]

        public required DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "El genero es obligatorio.")]
        [RegularExpression(@"^(Masculino|Femenino|Otro)$", ErrorMessage = "El género debe ser Masculino, Femenino u Otro.")]
        public required string Gender { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[0-9])(?=.*[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ])(?=.*[!@#$%^&*()_+\[\]{};':""\\|,.<>/?]).*$",
            ErrorMessage = "La contraseña debe ser alfanumérica, contener al menos una mayúscula y al menos un caracter especial.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        [MaxLength(20, ErrorMessage = "La contraseña debe tener como máximo 20 caracteres")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "La confirmación de contraseña es obligatoria.")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public required string ConfirmPassword { get; set; }


    }




}