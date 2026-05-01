namespace TiendaUCN.Domain.Models.Product
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public bool DeletedAt { get; set; } = false;
        public IEnumerable<src.Domain.Models.Product.Product>? Products { get; set; } = new List<src.Domain.Models.Product.Product>();
        public string? Description { get; set; }
    }

}