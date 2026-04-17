namespace TiendaUCN.src.Domain.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderCode { get; set; } = Guid.NewGuid().ToString("N").ToUpper()[..10];
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relación con User
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Relaciones
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}