namespace TiendaUCN.Domain.Models.Order
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int Quantity { get; set; }


        public decimal UnitPrice { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Subtotal { get; set; }
        // Relación con Order
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;


        public int ProductId { get; set; }
        public src.Domain.Models.Product.Product Product { get; set; } = null!;
    }
}