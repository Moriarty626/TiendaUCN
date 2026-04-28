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
        public bool DeletedAt { get; set; } = false;

        // Relación con Category
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        // Relación con Brand
        public int BrandId { get; set; }
        public Brand Brand { get; set; } = null!;

         // Relaciones
         public ICollection<Image> Images { get; set; } = new List<Image>();
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        
    }
}