namespace TiendaUCN.src.Domain.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        // Precio estático al momento de la compra (no cambia aunque el producto cambie de precio)
        public decimal UnitPrice { get; set; }
        public string ProductName { get; set; } = null!;

        // Relación con Order
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        // Relación con Product (referencia, pero el detalle es estático)
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}