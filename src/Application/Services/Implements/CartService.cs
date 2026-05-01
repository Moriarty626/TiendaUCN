using Microsoft.EntityFrameworkCore;
using TiendaUCN.Domain.Models.Cart;
using TiendaUCN.Infrastructure.Data.Migrations;
using TiendaUCN.src.Application.Services.Interfaces;

namespace TiendaUCN.src.Application.Services.Implements
{
    public class CartService(DataContext context) : ICartService
    {
        public async Task<string> CheckoutAsync(int userId)
        {
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var cartItems = await context.CartItems
                    .Include(ci => ci.Product)
                    .Include(ci => ci.Cart)
                    .Where(ci => ci.Cart.UserId == userId)
                    .ToListAsync();

                if (cartItems == null || !cartItems.Any())
                    throw new Exception("El carrito está vacío.");

                var order = new global::TiendaUCN.Domain.Models.Order.Order()
                {
                    UserId = userId,
                    OrderCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                    Total = cartItems.Sum(x => x.Product.Price * x.Quantity),
                    CreatedAt = DateTime.UtcNow,
                    OrderDetails = new List<global::TiendaUCN.Domain.Models.Order.OrderDetail>()
                };

                foreach (var item in cartItems)
                {
                    if (item.Product.Stock < item.Quantity)
                        throw new Exception($"Stock insuficiente para {item.Product.Name}");

                    order.OrderDetails.Add(new global::TiendaUCN.Domain.Models.Order.OrderDetail()
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product.Price,
                        ProductName = item.Product.Name,
                        Subtotal = item.Product.Price * item.Quantity
                    });

                    item.Product.Stock -= item.Quantity;
                }

                await context.Orders.AddAsync(order);
                context.CartItems.RemoveRange(cartItems);

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return $"Compra exitosa. Código: {order.OrderCode}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        public Task<Cart> GetCartByUserIdAsync(int userId) => throw new NotImplementedException();
        public Task<bool> AddItemToCartAsync(int userId, int productId, int quantity) => throw new NotImplementedException();
        public Task<bool> RemoveItemFromCartAsync(int userId, int productId) => throw new NotImplementedException();
    }
}