namespace TiendaUCN.src.Domain.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public bool DeletedAt { get; set; } = false;
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public string? Description { get; set; }
    }
    
}
