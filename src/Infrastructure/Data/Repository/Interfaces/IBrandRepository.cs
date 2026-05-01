using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaUCN.Domain.Models.Product;


namespace TiendaUCN.src.Domain.Models
{
    public interface IBrandRepository
    {
        Task<bool> ExistsNameAsync(string name);
        Task<bool> CreateBrandAsync(Brand brand);
        Task<bool> ExistsIdAsync(int id);
        Task<bool> UpdateNameAsync(int id, string name);
        Task<bool> UpdateDescriptionAsync(int id, string description);
        Task<bool> DeleteAsync(int id);
        Task<int> GetIdByNameAsync(string name);
    }
}