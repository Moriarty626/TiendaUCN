using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaUCN.src.Application.DTOs.ProductDTO
{
    public class SearchParamsDTO
    {
        [Required(ErrorMessage = "El número de página es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El número de página debe ser un número entero positivo.")]
        public required int PageNumber { get; set; }

        [Required(ErrorMessage = "El tamaño de página es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El tamaño de página debe ser un número entero positivo.")]
        public required int PageSize { get; set; }

        [MinLength(2, ErrorMessage = "El término de búsqueda debe tener al menos 2 caracteres.")]
        [MaxLength(25, ErrorMessage = "El término de búsqueda no puede exceder los 25 caracteres.")]
        public string? SearchTerm { get; set; }
    }
}