using System.ComponentModel.DataAnnotations;

namespace TiendaUCN.src.Application.Validators
{
    public class BirthDateValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime birthDate)
            {
                var today = DateTime.Today;
                var age = today.Year - birthDate.Year;

                if (birthDate.Date > today.AddYears(-age)) age--;

                if (age >= 18)
                    return ValidationResult.Success;

                return new ValidationResult("Debes ser mayor de 18 años para registrarte.");
            }

            return new ValidationResult("Fecha de nacimiento no válida.");
        }
    }
}