namespace TiendaUCN.Domain.Models.Product
{
    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public bool DeletedAt { get; set; } = false;
        public string? Description { get; set; }

        // Relaciones
        public IEnumerable<src.Domain.Models.Product.Product>? Products { get; set; } = new List<src.Domain.Models.Product.Product>();
    }
}