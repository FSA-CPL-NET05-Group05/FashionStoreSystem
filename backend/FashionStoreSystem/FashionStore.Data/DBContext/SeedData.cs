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


           // context.Database.EnsureCreated();

            await SeedRoles(roleManager);

            await SeedAdmin(userManager);

            await SeedCustomers(userManager);

            await SeedCategories(context);

            await SeedColors(context);

            await SeedSizes(context);

            await SeedProducts(context);

            await SeedProductSizes(context);

            await SeedFeedbacks(context);

            await SeedProductImages(context);
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
                    new Category { Name = "Áo Thun" },
                    new Category { Name = "Áo Sơ Mi" },
                    new Category { Name = "Quần Jean" },
                    new Category { Name = "Váy" },
                    new Category { Name = "Áo Khoác" }
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

        private static async Task SeedProductImages(ApplicationDBContext context)
        {
            if (!context.ProductImages.Any())
            {
                var products = await context.Products.ToListAsync();
                if (!products.Any()) return;

                var productImages = new List<ProductImage>();

                var imageMapping = new Dictionary<string, List<string>>
                {
                    // --- 1. NHÓM ÁO THUN ---
                    {
                        "Áo Thun Basic Cotton",
                        new List<string> {
                            "https://encrypted-tbn2.gstatic.com/shopping?q=tbn:ANd9GcToA8mpPPnN0KL2UFaeto26O6HaUF240ax4o5LqRk6Igb5VUYpI4j1Fdqpt5R-kaAVI2niphKUFeu8BkOeYmgJx3cs9VxZ-feuEeBXaEfLLybNjhZXvw5M-c0LgCweiDkHEeirDizeP&usqp=CAc",
                            "https://encrypted-tbn1.gstatic.com/shopping?q=tbn:ANd9GcT13C7WRAvjkSDs3Zw1pDvt4bdnYW0wBED0tc2cyRmKeDJXKjHOHSUP9fnhy-8J5Qd-58XdmXteFSwRchAZCa8YabiWUm86d6JmRmdDkbc4bkrLbHn1c91k1BgDNr2Er9v067vbXe3Z&usqp=CAc",
                            "https://encrypted-tbn0.gstatic.com/shopping?q=tbn:ANd9GcT5FQY9lPoLOQNyZv2vMElCLyn6LaV26WjPtFt0Wh3KcTENYp2LY27WDkzrWXNsBalw2TZmcSCKTghVFWfcQURpM3Hsa0kvLmlBDgDpAa7tj5a-w2U-kDJN5fGZ9N1dz_6zdApnMA1-&usqp=CAc",
                            "https://chodole.com/cdn/shop/products/CDL5_1_1024x1024.jpg?v=1586758906"
                        }
                    },
                    {
                        "Áo Thun Graphic Streetwear",
                        new List<string> {
                            "https://bizweb.dktcdn.net/100/287/440/products/ao-thun-in-hinh-tran-vien-mau-den-form-rong-local-brand-davies.jpg?v=1713778019587",
                            "https://bizweb.dktcdn.net/100/287/440/products/ao-thun-in-hinh-tran-vien-nam-nu-form-rong-den-trang-thun-cotton-local-brand-3-c62236a0-2bf7-4d48-82bd-51a7ce72e1cc.jpg?v=1729916781917",
                            "https://inaothunviet.com/wp-content/uploads/2025/03/ao_thun_in_hinh_thiet_ke_ca_map_streetwear_mau_den-300x300.jpg",
                            "https://bizweb.dktcdn.net/thumb/large/100/287/440/products/ao-thun-nam-nu-form-rong-davies-thiet-ke-hinh-in-chu-cach-dieu-dam-chat-duong-pho-co-gian-1.jpg?v=1758882635073"
                        }
                    },
                    {
                        "Áo Polo Nam Cổ Bẻ",
                        new List<string> {
                            "https://bizweb.dktcdn.net/thumb/1024x1024/100/290/346/products/ao-polo-nam-aristino-apsr10-anh-1.jpg",
                            "https://product.hstatic.net/1000369857/product/ao_ab19_xanh_den_1_c691c6dc769d483e860b6a1805be1b6f.jpg",
                            "https://bizweb.dktcdn.net/thumb/1024x1024/100/290/346/products/28-fada0d57-4b2e-4505-b6cf-f5ee145a8de5.jpg",
                            "https://bizweb.dktcdn.net/thumb/1024x1024/100/290/346/products/ao-polo-nam-aristino-aps011as1-1.jpg?v=1737459813673"
                        }
                    },

                    // --- 2. NHÓM SƠ MI ---
                    {
                        "Áo Sơ Mi Trắng Công Sở",
                        new List<string> {
                            "https://dongphuchaianh.vn/wp-content/uploads/2022/08/dong-phuc-ao-so-mi-nu-cong-so-mau-trang.jpg",
                            "https://dongphucbonmua.com/wp-content/uploads/2024/11/ao-so-mi-nam-dong-phuc-cong-so.jpg",
                            "https://js0fpsb45jobj.vcdn.cloud/storage/upload/media/3-trang-ae09043.jpg",
                            "https://pos.nvncdn.com/650b61-144700/art/artCT/20240517_rM6wXkIO.jpg"
                        }
                    },
                    {
                        "Áo Sơ Mi Flannel Caro",
                        new List<string> {
                            "https://maylanstore.com/wp-content/uploads/2024/03/ao-so-mi-flannel-ke-soc-10.jpg",
                            "https://aoxuanhe.com/upload/product/axh-148/ao-so-mi-flannel-caro-du-lich-nau.jpg",
                            "https://zeanus.vn/upload/product/zn-0124/ao-so-mi-nam-caro-do-du-lich-axh-114.jpg",
                            "https://img.lazcdn.com/g/p/5fec6fa5efd191b22442a2b40ef86380.jpg_720x720q80.jpg"
                        }
                    },

                    // --- 3. NHÓM QUẦN JEAN ---
                    {
                        "Quần Jean Slim Fit",
                        new List<string> {
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQhJ_VX8mMb7c6NRRBeiecDeBwp2lFi9zbY9w&s",
                            "https://pos.nvncdn.com/d0f3ca-7136/ps/20220915_9nYTI93O3l5MBMcc8mq3CSPS.jpg?v=1673542440",
                            "https://4men.com.vn/images/thumbs/2019/08/quan-jean-slimfit-xanh-bien-qj1653-14633-slide-products-5d64ff79d9e5f.JPG",
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ7bgTupkVkkzaWk5yzXSaTKoK-vvCi4Qa5Ow&s"
                        }
                    },
                    {
                        "Quần Jean Baggy Ống Rộng",
                        new List<string> {
                            "https://zizoou.com/cdn/shop/products/Quan-Baggy-Jean-nam-nu-2b-1-Quan-ong-rong-xanh-classic-ZiZoou-Store_4472x.jpg?v=1680283265",
                            "https://tashop.vn/templates/pictures/products/1584371768_quan-jean-baggy-ong-rong-77.jpg",
                            "https://lh6.googleusercontent.com/ZSswzo340XbSu_ee9GFyivmoGwrpOY093GE89isK79HONSP3_LR5Xk4GyDDUembNX3X2dD6GyEO01Lwfqc2GX2ktvWqdNuH_Vad4chxSgkwrRqWY996iDY2SfFKzyHstNrQ8XjS-5Bhna6YJwRvRSw",
                            "https://salt.tikicdn.com/cache/w300/ts/product/c0/b4/05/887b08ba90d1c69d3b00b603b59bc03f.jpg"
                        }
                    },
                    {
                        "Quần Short Jean Mùa Hè",
                        new List<string> {
                            "https://img.lazcdn.com/g/p/d2f0bedd2ddf9572cdb670d46270d96e.jpg_720x720q80.jpg",
                            "https://bizweb.dktcdn.net/thumb/grande/100/396/594/products/carbon-1.jpg?v=1711764388850",
                            "https://salt.tikicdn.com/cache/w300/ts/product/47/1c/a8/72c55776c8a530bbdafe1d191ce1b9e1.jpg",
                            "https://image.made-in-china.com/202f0j00isIcFfkgrSqb/Custom-Summer-Streetwear-Vintage-Short-Half-Pants-Loose-Work-Shorts-Fashion-Men-s-Jorts-Baggy-Denim-Jean-Shorts-Men.webp"
                        }
                    },

                    // --- 4. NHÓM VÁY ---
                    {
                        "Váy Hoa Nhí Vintage",
                        new List<string> {
                            "https://lamia.com.vn/storage/vay-hoa-nhi-1.jpg",
                            "https://salt.tikicdn.com/cache/550x550/ts/product/23/ec/c6/64e3cd85aec3192120813502d2df513b.jpg",
                            "https://img.riokupon.com/upload/images/2023/03/23/6f2e2598ac6f256f90810a008cec1c48.jpg",
                            "https://bloganchoi.com/wp-content/uploads/2018/04/vay-hoa-nhi.jpg"
                        }
                    },
                    {
                        "Đầm Dạ Hội Sang Trọng",
                        new List<string> {
                            "https://lh3.googleusercontent.com/proxy/281GP7V2VQQtjCwKT7G5_57B4A4Kn_Us0Ex6HxVrIwnxNcs9l7_jbfxwdpMzYfKPd_X_q0VZaqUBqu7SFJxTZ7F4WPvR2AiBhGUDZ4-bu5QslHleVB4uFofNznfPpUIMJZc",
                            "https://shopviets.com/wp-content/uploads/2023/05/Cac-mau-dam-da-hoi-sang-trong-mau-dam-da-hoi-moi-nhat-VDH33-3.jpg",
                            "https://product.hstatic.net/1000318527/product/141279554_2759566720950868_4151769136115659930_o_7f872a3e6d624b05a5ea7652f97d415f_master.jpg",
                            "https://img.lazcdn.com/g/ff/kf/S8602f76621804dd5b6ad14c403d32d450.jpg_720x720q80.jpg"
                        }
                    },

                     // --- 5. NHÓM ÁO KHOÁC ---
                    {
                        "Áo Khoác Denim",
                        new List<string> {
                            "https://bizweb.dktcdn.net/100/502/737/products/o1cn01da6ath1lkuuqdhr173965984.jpg?v=1734772093710",
                            "https://s3.ap-southeast-1.amazonaws.com/thegmen.vn/2023/3/1679305993206xqu3f.jpg",
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRg7OcXOTOU3QUyx1NZz0FYF21LeGutXltIIg&s",
                            "https://content.pancake.vn/1/s800x600/fwebp/24/ec/f7/55/240ea2b44625e7cca1b3e38716c5a710d2244909f1ae46f9a8310c4a-w:800-h:600-l:95724-t:image/jpeg.jpeg"
                        }
                    },
                    {
                        "Áo Hoodie Unisex",
                        new List<string> {
                            "https://dongphuchaianh.com/wp-content/uploads/2022/04/ao-khoac-ni-hoodie-unisex-mau-den.jpg",
                            "https://product.hstatic.net/200000755737/product/z6048366686279_258043bb579ab3dfbe984f8a5fef1734_1474b8de750b46ceac673e179ac84c5b.jpg",
                            "https://cache.maysoichivang.com/wp-content/uploads/2022/02/e5739d1ac504e86bd44fa447ad2ad7b6.jpg",
                            "https://product.hstatic.net/200000370449/product/hoodie_fire_den_sau_c133f948ef644d4a81a2592dac02ecc2_master.png"
                        }
                    },
                    {
                        "Áo Blazer Hàn Quốc",
                        new List<string> {
                            "https://bizweb.dktcdn.net/100/360/581/files/phoi-do-voi-blazer-nam-han-quoc-9.jpg?v=1710472586336",
                            "https://vn-test-11.slatic.net/p/592d3087d2be03a1889238d5e1e9b833.jpg",
                            "https://dukistore.vn/files/thumb/400/400//uploads/content/T04-2024/ao-blazer-nu-3-cuc-mau-kem-khong-co-BLU0109-2.jpg",
                            "https://lh7-us.googleusercontent.com/kJaE9IuTjYlnWcH771hqvXgtzj9h5a642ggD14XJogTwbAuAD5fGDASMflpzWfql4UD_J31Oc8Qg_uRTioA6ffTxctxblffJbdd9om0iOPFpdgRM2NUmutVhM93FtiB9uHrbBb9l-aCe1xJE5907dQ"
                        }
                    }
                };

                foreach (var product in products)
                {
                    if (imageMapping.ContainsKey(product.Name))
                    {
                        var specificImages = imageMapping[product.Name];

                        foreach (var url in specificImages)
                        {
                            productImages.Add(new ProductImage
                            {
                                ProductId = product.Id, 
                                Url = url
                            });
                        }
                    }
                }

                await context.ProductImages.AddRangeAsync(productImages);
                await context.SaveChangesAsync();
            }
        }
    }
}
