using Microsoft.EntityFrameworkCore;
using TiendaUCN.Domain.Models.Product;
using TiendaUCN.Infrastructure.Data.Migrations;
using TiendaUCN.src.Application.DTOs.CategoryDTO;
using TiendaUCN.src.Application.Services.Interfaces;

namespace TiendaUCN.Application.Services.Implements
{
    public class CategoryService(DataContext context) : ICategoryService
    {
        public async Task<List<CategoryResponseDTO>> GetAllAsync()
        {
            return await context.Categories
                .Select(c => new CategoryResponseDTO { Id = c.Id, Name = c.Name })
                .ToListAsync();
        }

        public async Task<CategoryResponseDTO?> GetByIdAsync(int id)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null) return null;

            return new CategoryResponseDTO { Id = category.Id, Name = category.Name };
        }

        public async Task<CategoryResponseDTO> CreateAsync(CategoryCreateDTO dto)
        {
            // Validamos que el nombre no esté repetido
            if (await context.Categories.AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower()))
                throw new Exception("Ya existe una categoría con este nombre.");

            var category = new Category { Name = dto.Name };
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();

            return new CategoryResponseDTO { Id = category.Id, Name = category.Name };
        }

        public async Task<bool> UpdateAsync(int id, CategoryCreateDTO dto)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null) return false;

            // Validamos que el nuevo nombre no choque con otra categoría existente
            if (await context.Categories.AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower() && c.Id != id))
                throw new Exception("Ya existe otra categoría con este nombre.");

            category.Name = dto.Name;
            context.Categories.Update(category);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null) return false;

            var tieneProductos = await context.Products.AnyAsync(p => p.CategoryId == id);
            if (tieneProductos)
                throw new Exception("No puedes eliminar esta categoría porque tiene productos asociados.");

            context.Categories.Remove(category);
            await context.SaveChangesAsync();

            return true;
        }
    }
}