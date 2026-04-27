using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaUCN.src.Domain.Models
{
    public interface ICategoryRepository
    {
        Task<bool> ExistsNameAsync(string name);
        Task<bool> CreateCategoryAsync(Category category);
        Task<bool> ExistsIdAsync(int id);
        Task<bool> UpdateNameAsync(int id, string name);
        Task<bool> UpdateDescriptionAsync(int id, string description);
        Task<bool> DeleteBrandAsync(int id);
        Task<int> GetBrandIdByNameAsync(string name);
    }
}