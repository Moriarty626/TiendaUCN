using Microsoft.EntityFrameworkCore;
using TiendaUCN.Domain.Models.Cart;
using TiendaUCN.Infrastructure.Data.Migrations;
using TiendaUCN.src.Application.Services.Interfaces;
using TiendaUCN.src.Domain.Models.Cart;

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
                    {
                        // Ajuste automático si el stock es menor a lo solicitado
                        item.Quantity = item.Product.Stock;
                        if (item.Quantity == 0)
                        {
                            throw new Exception($"El producto {item.Product.Name} ya no tiene stock disponible.");
                        }
                    }

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

                // Recalcular el total si hubo ajustes
                order.Total = order.OrderDetails.Sum(x => x.Subtotal);

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

        public async Task<Cart> GetCartByUserIdAsync(int userId)
        {
            var cart = await context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                context.Carts.Add(cart);
                await context.SaveChangesAsync();
            }

            // Calcular TotalPrice
            cart.TotalPrice = cart.Items.Sum(i => i.Subtotal);

            return cart;
        }

        public async Task<bool> AddItemToCartAsync(int userId, int productId, int quantity)
        {
            var cart = await GetCartByUserIdAsync(userId);
            var product = await context.Products.FindAsync(productId);

            if (product == null || !product.IsActive || product.DeletedAt)
                throw new Exception("El producto no existe o no está activo.");

            if (product.Stock < quantity)
                throw new Exception($"Stock insuficiente para {product.Name}. Disponible: {product.Stock}");

            var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity
                };
                context.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
                if (product.Stock < cartItem.Quantity)
                {
                    cartItem.Quantity = product.Stock; // Ajuste automático al stock disponible
                }
            }

            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveItemFromCartAsync(int userId, int productId)
        {
            var cart = await GetCartByUserIdAsync(userId);
            var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (cartItem != null)
            {
                context.CartItems.Remove(cartItem);
                return await context.SaveChangesAsync() > 0;
            }

            return false;
        }
    }
}