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

            await SeedFeedbacks(context);
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

                var tShirtCat = await context.Categories.FirstOrDefaultAsync(c => c.Name.Contains("Áo Thun"));
                var shirtCat = await context.Categories.FirstOrDefaultAsync(c => c.Name.Contains("Áo Sơ Mi"));
                var jeanCat = await context.Categories.FirstOrDefaultAsync(c => c.Name.Contains("Quần Jean"));
                var dressCat = await context.Categories.FirstOrDefaultAsync(c => c.Name.Contains("Váy"));
                var jacketCat = await context.Categories.FirstOrDefaultAsync(c => c.Name.Contains("Áo Khoác"));

                var products = new List<Product>();

                if (tShirtCat != null)
                {
                    products.Add(new Product { Name = "Áo Thun Basic Cotton", Description = "Áo thun trơn 100% cotton, thấm hút mồ hôi tốt.", Price = 150000, ImageUrl = "https://chodole.com/cdn/shop/products/CDL10_2_1024x1024.jpg?v=1586758482", CategoryId = tShirtCat.Id });
                    products.Add(new Product { Name = "Áo Thun Graphic Streetwear", Description = "Áo thun in hình nghệ thuật, phong cách đường phố.", Price = 220000, ImageUrl = "https://image.made-in-china.com/202f0j00pzucYePyYbog/Hip-Hop-Streetwear-T-Shirt-Men-Anime-Girl-Car-Graphic-Tees-Shirt-Harajuku-Casual-Cotton-Loose-Tshirt-Summer-Short-Sleeve-Tops.webp", CategoryId = tShirtCat.Id });
                    products.Add(new Product { Name = "Áo Polo Nam Cổ Bẻ", Description = "Áo Polo lịch sự, phù hợp đi làm và đi chơi.", Price = 250000, ImageUrl = "https://product.hstatic.net/1000369857/product/ao_ab19_xanh_den_1_c691c6dc769d483e860b6a1805be1b6f.jpg", CategoryId = tShirtCat.Id });
                }

                if (shirtCat != null)
                {
                    products.Add(new Product { Name = "Áo Sơ Mi Trắng Công Sở", Description = "Sơ mi trắng form chuẩn, chống nhăn nhẹ.", Price = 350000, ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcToXeJjGZ4RUsHoExIPkfM-r1KEHWjFK8EcMw&s", CategoryId = shirtCat.Id });
                    products.Add(new Product { Name = "Áo Sơ Mi Flannel Caro", Description = "Sơ mi kẻ caro phong cách vintage.", Price = 320000, ImageUrl = "https://product.hstatic.net/1000253775/product/ao-somi-tay-dai-icondenim-caro-flanel-2-tui-than-truoc__1__07455c4a4aa2461bbe4cead47316dd7e_1024x1024.jpg", CategoryId = shirtCat.Id });
                }

                if (jeanCat != null)
                {
                    products.Add(new Product { Name = "Quần Jean Slim Fit", Description = "Quần jean dáng ôm, co giãn tốt.", Price = 450000, ImageUrl = "https://s3.ap-southeast-1.amazonaws.com/thegmen.vn/2024/4/1714204044656je5i7.jpg", CategoryId = jeanCat.Id });
                    products.Add(new Product { Name = "Quần Jean Baggy Ống Rộng", Description = "Quần ống rộng thoải mái, che khuyết điểm.", Price = 480000, ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQBtTCTiID4lKsr_Cl74w3KPzPAdudPCtAAog&s", CategoryId = jeanCat.Id });
                    products.Add(new Product { Name = "Quần Short Jean Mùa Hè", Description = "Quần short năng động cho ngày hè.", Price = 280000, ImageUrl = "https://vn-live-01.slatic.net/p/94ff3f512318a1a3ed63ba6b6363ddd0.jpg", CategoryId = jeanCat.Id });
                }

                if (dressCat != null)
                {
                    products.Add(new Product { Name = "Váy Hoa Nhí Vintage", Description = "Váy hoa nhẹ nhàng, nữ tính.", Price = 390000, ImageUrl = "https://file.hstatic.net/1000317075/file/vay-hoa-nhi__17_.jpeg_1548d391791c4f6cb7383ce0c680d4cd.jpg", CategoryId = dressCat.Id });
                    products.Add(new Product { Name = "Đầm Dạ Hội Sang Trọng", Description = "Thiết kế xẻ tà, tôn dáng, phù hợp dự tiệc.", Price = 850000, ImageUrl = "https://bizweb.dktcdn.net/100/368/426/products/dam-da-hoi-co-yem-dep.jpg?v=1750250243937", CategoryId = dressCat.Id });
                }

                if (jacketCat != null)
                {
                    products.Add(new Product { Name = "Áo Khoác Denim", Description = "Áo khoác bò bụi bặm, cá tính.", Price = 550000, ImageUrl = "https://product.hstatic.net/200000370509/product/7491_e8bb8aadee1624af4aa969e527d89b3c_6761a257c0b64a7babea97c14768ba80_76a1292d723a4e8c8002f49999419a21_master.jpg", CategoryId = jacketCat.Id });
                    products.Add(new Product { Name = "Áo Hoodie Unisex", Description = "Áo nỉ có mũ form rộng, ấm áp.", Price = 300000, ImageUrl = "https://img.lazcdn.com/g/p/5e0e21fc98707c05f10974ef721cc7bd.jpg_720x720q80.jpg", CategoryId = jacketCat.Id });
                    products.Add(new Product { Name = "Áo Blazer Hàn Quốc", Description = "Áo khoác blazer thanh lịch, dễ phối đồ.", Price = 650000, ImageUrl = "https://img.lazcdn.com/g/p/24c24ff47e1f5e61727e887d5030d32d.jpg_720x720q80.jpg", CategoryId = jacketCat.Id });
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

        private static async Task SeedFeedbacks(ApplicationDBContext context)
        {
            if (!context.Feedbacks.Any())
            {
                var products = await context.Products.ToListAsync();
                var customers = await context.Users.Where(u => u.Email.Contains("customer")).ToListAsync();

                if (!products.Any() || !customers.Any()) return;

                var feedbacks = new List<Feedback>();
                var random = new Random();

                var goodComments = new[]
                {
                    "Sản phẩm tuyệt vời, rất đáng tiền!",
                    "Giao hàng siêu nhanh, đóng gói cẩn thận.",
                    "Vải đẹp, mặc rất mát, sẽ ủng hộ shop tiếp.",
                    "Đúng như mô tả, cho shop 5 sao.",
                    "Màu sắc bên ngoài đẹp hơn trong ảnh.",
                    "Form áo chuẩn, mặc lên rất tôn dáng.",
                    "Nhân viên tư vấn nhiệt tình, hàng chất lượng.",
                    "Rất hài lòng với trải nghiệm mua hàng."
                };

                foreach (var product in products)
                {
                    int reviewCount = random.Next(3, 8);

                    for (int i = 0; i < reviewCount; i++)
                    {
                        var customer = customers[random.Next(customers.Count)];

                        int rating = random.Next(4, 6);

                        feedbacks.Add(new Feedback
                        {
                            ProductId = product.Id,
                            UserId = customer.Id,
                            Rating = rating,
                            Comment = goodComments[random.Next(goodComments.Length)],
                            CreatedDate = DateTime.Now.AddDays(-random.Next(0, 60)).AddHours(random.Next(0, 24))
                        });
                    }
                }

                await context.Feedbacks.AddRangeAsync(feedbacks);
                await context.SaveChangesAsync();
            }
        }
    }
}
