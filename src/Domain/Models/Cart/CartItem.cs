namespace TiendaUCN.src.Domain.Models.Cart
{
    public class CartItem
    {
        public int Id { get; set; }

        // Relación con el Carrito (Como están en la misma carpeta, Rider lo detecta solo)
        public int CartId { get; set; }
        public virtual TiendaUCN.Domain.Models.Cart.Cart Cart { get; set; } = null!;

        // Relación con el Producto (Ruta completa y absoluta a la clase para evitar confusiones)
        public int ProductId { get; set; }
        public virtual TiendaUCN.src.Domain.Models.Product.Product Product { get; set; } = null!;

        public int Quantity { get; set; }

        // Propiedad calculada
        public decimal Subtotal => Product != null ? Product.Price * Quantity : 0;
    }
}