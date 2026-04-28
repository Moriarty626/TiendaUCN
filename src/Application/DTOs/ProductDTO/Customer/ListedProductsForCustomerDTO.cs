using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaUCN.src.Application.DTOs.ProductDTO.Customer
{
    public class ListedProductsForCustomerDTO
    {
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int PagesSize { get; set; }
        public int ProductPage { get; set; }
        public int CurrentPage { get; set; }

        public List<ProductForCustomerDTO> Products { get; set; } = new List<ProductForCustomerDTO>();

    }
}