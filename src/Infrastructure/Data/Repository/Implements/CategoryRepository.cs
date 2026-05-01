using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaUCN.Domain.Models.Product;
using TiendaUCN.Infrastructure.Data.Migrations;
using TiendaUCN.src.Domain.Models;

namespace TiendaUCN.src.Infrastructure.Data.Repository.Implements
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext _context;
        public CategoryRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _context.Categories
                .Where(b => b.Id == id)
                .ExecuteUpdateAsync(b => b.SetProperty(x => x.DeletedAt, true));

            return result > 0;
        }

        public async Task<bool> ExistsIdAsync(int id)
        {
            return await _context.Categories
                .AnyAsync(b => b.Id == id && b.DeletedAt == false);
        }

        public async Task<bool> ExistsNameAsync(string name)
        {
            return await _context.Categories
                .AnyAsync(b =>
                    b.Name.ToLower() == name.ToLower() &&
                    b.DeletedAt == false);
        }

        public async Task<int> GetIdByNameAsync(string name)
        {
            return await _context.Categories
                .Where(b => b.Name.ToLower() == name.ToLower() && !b.DeletedAt)
                .Select(b => b.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateDescriptionAsync(int id, string description)
        {
            var result = await _context.Categories
                .Where(b => b.Id == id)
                .ExecuteUpdateAsync(b => b.SetProperty(x => x.Description, description));

            return result > 0;
        }

        public async Task<bool> UpdateNameAsync(int id, string name)
        {
            var result = await _context.Categories
                .Where(b => b.Id == id)
                .ExecuteUpdateAsync(b => b.SetProperty(x => x.Name, name));
            return result > 0;
        }
    }
}