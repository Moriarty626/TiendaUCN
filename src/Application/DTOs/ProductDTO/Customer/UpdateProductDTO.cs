using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace TiendaUCN.src.Application.DTOs.ProductDTO.Customer
{
    public class UpdateProductDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? Stock { get; set; }
        public string? CategoryName { get; set; }
        public string? BrandName { get; set; }
        public List<IFormFile>? ImagesFiles { get; set; }
    }
}