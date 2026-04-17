namespace TiendaUCN.src.Domain.Models
{
    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public DateTime? DeletedAt { get; set; }

        // Relaciones
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}