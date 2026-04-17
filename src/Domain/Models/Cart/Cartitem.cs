using TiendaUCN.Models;
namespace TiendaUCN.src.Domain.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        // Relación con User
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Relación con Product
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}