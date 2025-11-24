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
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<AccountLockHistory> AccountLockHistories { get; set; }


        public DbSet<Size> Sizes { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<Color> Colors { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // --- Category ---
            builder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(100);
            });

            // --- Product ---
            builder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(200);
                entity.Property(e => e.Price)
                      .HasColumnType("decimal(18,2)");

                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // --- CartItem ---
            builder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(c => c.User)
                      .WithMany(u => u.CartItems)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // --- Order ---
            builder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalAmount)
                      .HasColumnType("decimal(18,2)");

                entity.HasOne(o => o.User)
                      .WithMany(u => u.Orders)
                      .HasForeignKey(o => o.UserId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // --- Size ---
            builder.Entity<Size>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(20);
            });

            // --- ProductSize ---
            builder.Entity<ProductSize>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Stock)
                      .IsRequired();

                entity.HasOne(ps => ps.Product)
                      .WithMany(p => p.ProductSizes)
                      .HasForeignKey(ps => ps.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ps => ps.Size)
                      .WithMany(s => s.ProductSizes)
                      .HasForeignKey(ps => ps.SizeId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ps => ps.Color)                 
                      .WithMany(c => c.ProductSizes)
                      .HasForeignKey(ps => ps.ColorId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(ps => new { ps.ProductId, ps.SizeId, ps.ColorId })
                      .IsUnique();   
            });


            // --- OrderDetail ---
            builder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price)
                      .HasColumnType("decimal(18,2)");

                entity.HasOne(od => od.Order)
                      .WithMany(o => o.OrderDetails)
                      .HasForeignKey(od => od.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(od => od.Product)
                      .WithMany(p => p.OrderDetails)
                      .HasForeignKey(od => od.ProductId);

                entity.HasOne(od => od.Size)
                      .WithMany()                    
                      .HasForeignKey(od => od.SizeId);

                entity.HasOne(od => od.Color)        
                      .WithMany()
                      .HasForeignKey(od => od.ColorId);
            });


            // --- Feedback ---
            builder.Entity<Feedback>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(f => f.User)
                      .WithMany(u => u.Feedbacks)
                      .HasForeignKey(f => f.UserId);

                entity.HasOne(f => f.Product)
                      .WithMany(p => p.Feedbacks)
                      .HasForeignKey(f => f.ProductId);
            });



            // Cấu hình AccountLockHistory
            builder.Entity<AccountLockHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Reason).HasMaxLength(500);
                entity.Property(e => e.TargetUserId).IsRequired();
                entity.Property(e => e.PerformedByUserId).IsRequired();

                // Quan hệ với AppUser
                entity.HasOne(e => e.TargetUser)
                      .WithMany()
                      .HasForeignKey(e => e.TargetUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.PerformedByUser)
                      .WithMany()
                      .HasForeignKey(e => e.PerformedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });



            // --- Color ---
            builder.Entity<Color>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(50);
            });

            // --- ProductImage ---
            builder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Url)
                      .IsRequired();

                entity.HasOne(d => d.Product)
                      .WithMany(p => p.Images) 
                      .HasForeignKey(d => d.ProductId)
                      .OnDelete(DeleteBehavior.Cascade); 
            });


        }

    }
}
