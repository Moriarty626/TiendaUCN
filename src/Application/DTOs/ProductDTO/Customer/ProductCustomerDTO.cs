namespace TiendaUCN.src.Application.DTOs.ProductDTO.Customer
{
    public class ProductForCustomerDTO
    {
        public required int Id { get; set; }
        public required string Name { get; set; } = null!;
        public required string Description { get; set; } = null!;
        public required string Price { get; set; }
        public required string StockIndicator { get; set; }
        public required string MainImagesURL { get; set; }
        public required string CategoryName { get; set; } = null!;
        public required string BrandName { get; set; } = null!;
    }
}