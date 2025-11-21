using FashionStore.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.DBContext
{
    public static class SeedData
    {
        public static async Task Seed(IServiceProvider serviceProvider)
        {

            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = serviceProvider.GetRequiredService<ApplicationDBContext>();


            context.Database.EnsureCreated();

            await SeedRoles(roleManager);

            await SeedAdmin(userManager);

            await SeedCustomers(userManager);

            await SeedCategories(context);

            await SeedColors(context);

            await SeedSizes(context);

            await SeedProducts(context);

            await SeedProductSizes(context);
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "Customer" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task SeedAdmin(UserManager<AppUser> userManager)
        {
            var adminUserName = "admin@12345";
            var adminEmail = "admin@fashionstore.com";
            var adminFullName = "System Administrator";
            var adminAddress = "Vietnam, Hanoi";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var newAdmin = new AppUser
                {
                    UserName = adminUserName,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = adminFullName,
                    Address = adminAddress
                };

                var result = await userManager.CreateAsync(newAdmin, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
            else
            {
                adminUser.UserName = adminUserName;
                adminUser.FullName = adminFullName;
                adminUser.Address = adminAddress;
                await userManager.UpdateAsync(adminUser);

                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

        private static async Task SeedCustomers(UserManager<AppUser> userManager)
        {
            for (int i = 1; i <= 5; i++)
            {
                var customerUserName = $"customer{i}@12345";
                var customerFullName = $"Customer Number {i}";
                var customerEmail = $"customer{i}@fashionstore.com";
                var customerAddress = $"Vietnam, City {i} ";

                var existingCustomer = await userManager.FindByEmailAsync(customerEmail);

                if (existingCustomer == null)
                {
                    var newCustomer = new AppUser
                    {
                        UserName = customerUserName,
                        Email = customerEmail,
                        EmailConfirmed = true,
                        FullName = customerFullName,
                        Address = customerAddress
                    };

                    var result = await userManager.CreateAsync(newCustomer, "Customer@123");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newCustomer, "Customer");
                    }
                }
                else
                {

                    existingCustomer.FullName = customerFullName;
                    existingCustomer.Address = customerAddress;

                    await userManager.UpdateAsync(existingCustomer);

                    if (!await userManager.IsInRoleAsync(existingCustomer, "Customer"))
                    {
                        await userManager.AddToRoleAsync(existingCustomer, "Customer");
                    }
                }    
            }
        }

        private static async Task SeedCategories(ApplicationDBContext context)
        {
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Áo Thun (T-Shirt)" },
                    new Category { Name = "Áo Sơ Mi (Shirt)" },
                    new Category { Name = "Quần Jean (Jeans)" },
                    new Category { Name = "Váy (Dress)" },
                    new Category { Name = "Áo Khoác (Jacket)" }
                };
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedColors(ApplicationDBContext context)
        {
            if (!context.Colors.Any())
            {
                var colors = new List<Color>
                {
                    new Color { Name = "Đen", Code = "#000000" },
                    new Color { Name = "Trắng", Code = "#FFFFFF" },
                    new Color { Name = "Xám", Code = "#808080" }
                };
                await context.Colors.AddRangeAsync(colors);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedSizes(ApplicationDBContext context)
        {
            if (!context.Sizes.Any())
            {
                var sizes = new List<Size>
                {
                    new Size { Name = "S" },
                    new Size { Name = "M" },
                    new Size { Name = "L" },
                    new Size { Name = "XL" }
                };
                await context.Sizes.AddRangeAsync(sizes);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedProducts(ApplicationDBContext context)
        {
            if (!context.Products.Any())
            {

                var aoThunCat = await context.Categories.FirstOrDefaultAsync(c => c.Name.Contains("Áo Thun"));
                var quanJeanCat = await context.Categories.FirstOrDefaultAsync(c => c.Name.Contains("Quần Jean"));

                var products = new List<Product>();

                if (aoThunCat != null)
                {
                    products.Add(new Product
                    {
                        Name = "Áo Thun Basic Cotton",
                        Description = "Áo thun chất liệu 100% cotton thoáng mát",
                        Price = 150000,
                        ImageUrl = "https://placehold.co/600x400?text=T-Shirt", 
                        CategoryId = aoThunCat.Id
                    });
                    products.Add(new Product
                    {
                        Name = "Áo Thun Graphic In Hình",
                        Description = "Áo thun in hình nghệ thuật phong cách đường phố",
                        Price = 200000,
                        ImageUrl = "https://placehold.co/600x400?text=Graphic+Tee",
                        CategoryId = aoThunCat.Id
                    });
                }

                if (quanJeanCat != null)
                {
                    products.Add(new Product
                    {
                        Name = "Quần Jean Slim Fit",
                        Description = "Quần bò dáng ôm, co giãn nhẹ",
                        Price = 450000,
                        ImageUrl = "https://placehold.co/600x400?text=Jeans",
                        CategoryId = quanJeanCat.Id
                    });
                }

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedProductSizes(ApplicationDBContext context)
        {
            if (!context.ProductSizes.Any())
            {
                var products = await context.Products.ToListAsync();
                var colors = await context.Colors.ToListAsync();
                var sizes = await context.Sizes.ToListAsync();

                if (!products.Any() || !colors.Any() || !sizes.Any()) return;

                var productSizes = new List<ProductSize>();
                var random = new Random();

                foreach (var product in products)
                {
                    foreach (var color in colors)
                    {
                        foreach (var size in sizes)
                        {
  
                            productSizes.Add(new ProductSize
                            {
                                ProductId = product.Id,
                                ColorId = color.Id,
                                SizeId = size.Id,
                                Stock = random.Next(20, 100) 
                            });
                        }
                    }
                }

                await context.ProductSizes.AddRangeAsync(productSizes);
                await context.SaveChangesAsync();
            }
        }
    }
}
