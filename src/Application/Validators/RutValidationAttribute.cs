using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TiendaUCN.src.Application.Validators
{
    public class RutValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return ValidationResult.Success;

            string rut = value.ToString()!.Replace(".", "").ToUpper();

            if (!Regex.IsMatch(rut, @"^\d{7,8}-[0-9K]$"))
                return new ValidationResult("Formato de RUT inválido.");

            string[] parts = rut.Split('-');
            if (!int.TryParse(parts[0], out int number))
                return new ValidationResult("Cuerpo del RUT inválido.");

            char dv = parts[1][0];

            if (CalculateDV(number) == dv)
                return ValidationResult.Success;

            return new ValidationResult(ErrorMessage ?? "El RUT no es válido.");
        }

        private char CalculateDV(int rut)
        {
            int m = 0, s = 1;
            for (; rut != 0; rut /= 10)
                s = (s + rut % 10 * (9 - m++ % 6)) % 11;
            return (char)(s != 0 ? s + 47 : 75);
        }
    }
}