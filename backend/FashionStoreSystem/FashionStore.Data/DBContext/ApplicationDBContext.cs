using FashionStore.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.DBContext
{
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // --- Cấu hình cho bảng Category ---
            builder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            // --- Cấu hình cho bảng Product ---
            builder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)"); // Định dạng tiền tệ tránh lỗi 

                // Mối quan hệ 1-N: Một Category có nhiều Product
                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict); // Xóa Category không xóa Product (để an toàn dữ liệu)
            });

            // --- Cấu hình cho bảng CartItem ---
            builder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                // 1 User có nhiều CartItem
                entity.HasOne(c => c.User)
                      .WithMany(u => u.CartItems)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade); // Xóa User thì xóa luôn Giỏ hàng của họ
            });

            // --- Cấu hình cho bảng Order ---
            builder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");

                // 1 User có nhiều Order
                // Vì UserId trong Order cho phép null (Guest), nên mối quan hệ này là Optional
                entity.HasOne(o => o.User)
                      .WithMany(u => u.Orders)
                      .HasForeignKey(o => o.UserId)
                      .IsRequired(false) // Cho phép null
                      .OnDelete(DeleteBehavior.SetNull); // Xóa User thì giữ lại đơn hàng, set UserId = null
            });

            // --- Cấu hình cho bảng OrderDetail ---
            builder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");

                // 1 Order có nhiều OrderDetail
                entity.HasOne(od => od.Order)
                      .WithMany(o => o.OrderDetails)
                      .HasForeignKey(od => od.OrderId)
                      .OnDelete(DeleteBehavior.Cascade); // Xóa Order thì xóa chi tiết đơn

                // 1 Product có thể nằm trong nhiều OrderDetail
                entity.HasOne(od => od.Product)
                      .WithMany(p => p.OrderDetails)
                      .HasForeignKey(od => od.ProductId);
            });

            // --- Cấu hình cho bảng Feedback ---
            builder.Entity<Feedback>(entity =>
            {
                entity.HasKey(e => e.Id);

                // 1 User có nhiều Feedback
                entity.HasOne(f => f.User)
                      .WithMany(u => u.Feedbacks)
                      .HasForeignKey(f => f.UserId);

                // 1 Product có nhiều Feedback
                entity.HasOne(f => f.Product)
                      .WithMany(p => p.Feedbacks)
                      .HasForeignKey(f => f.ProductId);
            });
        }
    }
}
