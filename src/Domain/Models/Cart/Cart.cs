using TiendaUCN.src.Domain.Models.Cart;

namespace TiendaUCN.Domain.Models.Cart
{
    public class Cart
    {
        public int Id { get; set; }

        // Relación con el Usuario
        public int UserId { get; set; }
        public virtual User.User User { get; set; } = null!;

        // Colección de ítems dentro del carrito
        public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();

        // Propiedad calculada para mostrar el total en el GET
        public decimal TotalPrice { get; set; }
    }
}