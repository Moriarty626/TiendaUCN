namespace TiendaUCN.src.Domain.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }

        // Relación con Category
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        // Relación con Brand
        public int BrandId { get; set; }
        public Brand Brand { get; set; } = null!;

        // Relación con Image (1 a 1)
        public Image? Image { get; set; }

        // Relaciones
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        
    }
}