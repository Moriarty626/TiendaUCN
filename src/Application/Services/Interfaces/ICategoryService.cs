using TiendaUCN.src.Application.DTOs.CategoryDTO;

namespace TiendaUCN.src.Application.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryResponseDTO>> GetAllAsync();
        Task<CategoryResponseDTO?> GetByIdAsync(int id);
        Task<CategoryResponseDTO> CreateAsync(CategoryCreateDTO dto);
        Task<bool> UpdateAsync(int id, CategoryCreateDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}