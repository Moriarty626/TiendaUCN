using Microsoft.EntityFrameworkCore;

using TiendaUCN.src.Domain.Models;

namespace TiendaUCN.src.Infrastructure.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Brand> Brands { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Image> Images { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public DbSet<JwtBlacklist> JwtBlacklist { get; set; } = null!;
        public DbSet<VerificationCode> VerificationCodes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Role ──────────────────────────────────────────────
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Name).IsRequired().HasMaxLength(50);
                entity.HasIndex(r => r.Name).IsUnique();
            });

            // ── User ──────────────────────────────────────────────
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Rut).IsRequired().HasMaxLength(12);
                entity.HasIndex(u => u.Rut).IsUnique();
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.PhoneNumber).HasMaxLength(20);
                entity.Property(u => u.Gender).HasMaxLength(20);
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // User → Role (N:1)
                entity.HasOne(u => u.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ── Category ──────────────────────────────────────────
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(c => c.Name).IsUnique();
            });

            // ── Brand ─────────────────────────────────────────────
            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(b => b.Name).IsUnique();
            });

            // ── Product ───────────────────────────────────────────
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(150);
                entity.Property(p => p.Description).HasMaxLength(500);
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Product → Category (N:1)
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Product → Brand (N:1)
                entity.HasOne(p => p.Brand)
                    .WithMany(b => b.Products)
                    .HasForeignKey(p => p.BrandId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ── Image ─────────────────────────────────────────────
            modelBuilder.Entity<Image>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.ImageUrl).IsRequired();
                entity.Property(i => i.PublicId).IsRequired();

                // Image → Product (1:1)
                entity.HasOne(i => i.Product)
                    .WithMany(p => p.Images)
                    .HasForeignKey(i => i.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ── CartItem ──────────────────────────────────────────
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(ci => ci.Id);
                entity.Property(ci => ci.Quantity).IsRequired();

                // CartItem → User (N:1)
                entity.HasOne(ci => ci.User)
                    .WithMany(u => u.CartItems)
                    .HasForeignKey(ci => ci.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // CartItem → Product (N:1)
                entity.HasOne(ci => ci.Product)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(ci => ci.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Un usuario no puede tener el mismo producto dos veces en el carrito
                entity.HasIndex(ci => new { ci.UserId, ci.ProductId }).IsUnique();
            });

            // ── Order ─────────────────────────────────────────────
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.OrderCode).IsRequired().HasMaxLength(20);
                entity.HasIndex(o => o.OrderCode).IsUnique();
                entity.Property(o => o.Total).HasColumnType("decimal(18,2)");
                entity.Property(o => o.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Order → User (N:1)
                entity.HasOne(o => o.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ── OrderDetail ───────────────────────────────────────
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(od => od.Id);
                entity.Property(od => od.Quantity).IsRequired();
                entity.Property(od => od.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(od => od.ProductName).IsRequired().HasMaxLength(150);

                // OrderDetail → Order (N:1)
                entity.HasOne(od => od.Order)
                    .WithMany(o => o.OrderDetails)
                    .HasForeignKey(od => od.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                // OrderDetail → Product (N:1)
                entity.HasOne(od => od.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(od => od.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ── JwtBlacklist ──────────────────────────────────────
            modelBuilder.Entity<JwtBlacklist>(entity =>
            {
                entity.HasKey(j => j.Id);
                entity.Property(j => j.TokenId).IsRequired();
                entity.HasIndex(j => j.TokenId).IsUnique();
                entity.Property(j => j.ExpiresAt).IsRequired();
                entity.Property(j => j.InvalidatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        }
    }
}