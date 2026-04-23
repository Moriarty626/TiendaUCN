namespace TiendaUCN.Application.DTOs.ProductDTO;

public class ProductCreateDTO
{
    public required string Name { get; set; }
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
    public IFormFile? Image { get; set; }
}