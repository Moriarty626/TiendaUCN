using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaUCN.src.Application.DTOs.ProductDTO.Admin
{
    public class ProductForAdminDTO
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public string? MainImagesURL { get; set; }
        public required string Price { get; set; }
        public required string StockIndicator { get; set; }
        public int Stock { get; set; }
        public required string Available { get; set; }
    }
}