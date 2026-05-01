using System.ComponentModel.DataAnnotations;

namespace TiendaUCN.src.Application.DTOs.CategoryDTO
{
    public class CategoryCreateDTO
    {
        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres.")]
        public string Name { get; set; } = null!;
    }
}