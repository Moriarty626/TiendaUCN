using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaUCN.Domain.Models.Product;
using TiendaUCN.Infrastructure.Data.Migrations;
using TiendaUCN.src.Application.DTOs.ProductDTO;
using TiendaUCN.src.Domain.Models;
using TiendaUCN.src.Domain.Models.Product;

namespace TiendaUCN.src.Infrastructure.Data.Repository.Implements
{
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _context;
        public ProductRepository(DataContext context)
        {
            _context = context;
        }

        public Task<bool> CreateProductAsync(Product product)
        {
            _context.Products.Add(product);
            return Task.FromResult(_context.SaveChanges() > 0);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _context.Products
                .Where(p => p.Id == id && p.DeletedAt == false)
                .ExecuteUpdateAsync(p =>
                    p.SetProperty(p => p.DeletedAt, true)) > 0;
        }

        public async Task<bool> ExistsIdAsync(int id)
        {
            return await _context.Products
                .AnyAsync(p =>
                p.Id == id &&
                p.DeletedAt == false);
        }

        public async Task<bool> ExistsIdCustomerAsync(int id)
        {
            return await _context.Products
                .AnyAsync(p =>
                p.Id == id &&
                p.DeletedAt == false &&
                p.IsActive == true);
        }

        public async Task<bool> ExistsNameAndBrandAsync(string name, string brandName)
        {
            return await _context.Products
                .Include(p => p.Brand)
                .AnyAsync(p =>
                p.Name.ToLower() == name.ToLower() &&
                p.Brand.Name.ToLower() == brandName.ToLower() &&
                p.DeletedAt == false &&
                p.Brand.DeletedAt == false);
        }

        public Task<(IEnumerable<Product> products, int totalCount)> GetFilteredAdminAsync(SearchParamsDTO searchParams)
        {
            throw new NotImplementedException();
        }

        public async Task<(IEnumerable<Product> products, int totalCount)> GetFilteredCustomerAsync(SearchParamsDTO searchParams)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images.Take(1))
                .Where(p => p.DeletedAt == false && p.IsActive == true)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            {
                var search = searchParams.SearchTerm.Trim().ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(search) ||
                    p.Description.ToLower().Contains(search) ||
                    p.Category.Name.ToLower().Contains(search) ||
                    (p.Category.Description != null && p.Category.Description.ToLower().Contains(search)) ||
                    p.Brand.Name.ToLower().Contains(search) ||
                     (p.Brand.Description != null && p.Brand.Description.ToLower().Contains(search)) ||
                    p.Price.ToString().Contains(search) ||
                    p.Stock.ToString().Contains(search));
            }
            int totalCount = await query.CountAsync();

            var products = await query
                 .OrderByDescending(p => p.CreatedAt)
                .Skip((searchParams.PageNumber - 1) * searchParams.PageSize)
                .Take(searchParams.PageSize)
                .ToArrayAsync();
            return (products, totalCount);
        }



        public async Task<Product?> GetProductIdAdminAsync(int id)
        {
            return await _context.Products
               .Include(p => p.Category)
               .Include(p => p.Brand)
               .Include(p => p.Images)
               .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == false);
        }

        public async Task<Product?> GetProductIdCustomerAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == false && p.IsActive == true);
        }

        public async Task<string?> GetStatusAsync(int id)
        {
            return await _context.Products
                .Where(p => p.Id == id && p.DeletedAt == false)
                .Select(p => p.IsActive ? "Activo" : "Inactivo")
                .FirstOrDefaultAsync();
        }

        public async Task<bool> SwitchStatusAsync(int id)
        {
            return await _context.Products
                .Where(p => p.Id == id && p.DeletedAt == false)
                .ExecuteUpdateAsync(p => p.SetProperty(x => x.IsActive, p => !p.IsActive)) > 0;
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            return await _context.Products
                .Where(p => p.Id == product.Id && p.DeletedAt == false)
                .ExecuteUpdateAsync(p =>
                    p.SetProperty(x => x.Name, product.Name)
                     .SetProperty(x => x.Description, product.Description)
                     .SetProperty(x => x.Price, product.Price)
                     .SetProperty(x => x.Stock, product.Stock)
                     .SetProperty(x => x.CategoryId, product.CategoryId)
                     .SetProperty(x => x.BrandId, product.BrandId)) > 0;
        }
    }
}