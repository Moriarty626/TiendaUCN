using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaUCN.Domain.Models.Product;
using TiendaUCN.src.Application.DTOs.ProductDTO;

namespace TiendaUCN.src.Domain.Models
{
    public interface IProductRepository
    {
        Task<bool> ExistsNameAndBrandAsync(string name, string brandName);
        Task<bool> CreateProductAsync(Product.Product product);
        Task<bool> ExistsIdAsync(int id);
        Task<bool> SwitchStatusAsync(int id);
        Task<string?> GetStatusAsync(int id);
        Task<bool> ExistsIdCustomerAsync(int id);
        Task<Product.Product?> GetProductIdCustomerAsync(int id);
        Task<Product.Product?> GetProductIdAdminAsync(int id);
        Task<bool> DeleteAsync(int id);

        Task<(IEnumerable<Product.Product> products, int totalCount)> GetFilteredAdminAsync(SearchParamsDTO searchParams);
        Task<(IEnumerable<Product.Product> products, int totalCount)> GetFilteredCustomerAsync(SearchParamsDTO searchParams);

        Task<bool> UpdateProductAsync(Product.Product product);
    }

}