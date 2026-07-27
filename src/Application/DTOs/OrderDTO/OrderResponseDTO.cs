namespace TiendaUCN.src.Application.DTOs.OrderDTO
{
    public class OrderResponseDTO
    {
        public int Id { get; set; }
        public string OrderCode { get; set; } = null!;
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderDetailResponseDTO> Details { get; set; } = new();
    }

    public class OrderDetailResponseDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class CreateOrderDTO
    {
        public List<CreateOrderItemDTO> Items { get; set; } = new();
    }

    public class CreateOrderItemDTO
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }
}