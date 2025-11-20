using FashionStore.Data.Models;
using Microsoft.AspNetCore.Identity;
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
            // Lấy các service cần thiết từ Dependency Injection container
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = serviceProvider.GetRequiredService<ApplicationDBContext>();

            // Đảm bảo database đã được tạo
            context.Database.EnsureCreated();

            // 1. Seed Roles (Admin & Customer)
            await SeedRoles(roleManager);

            // 2. Seed Admin User
            await SeedAdmin(userManager);

            // 3. Seed Customer Users
            await SeedCustomers(userManager);
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "Customer" };

            foreach (var roleName in roleNames)
            {
                // Kiểm tra xem Role đã tồn tại chưa, nếu chưa thì tạo mới
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

            // Kiểm tra xem admin đã tồn tại chưa
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

                // Tạo user với mật khẩu mạnh (Identity yêu cầu mật khẩu mạnh: Chữ hoa, thường, số, ký tự đặc biệt)
                var result = await userManager.CreateAsync(newAdmin, "Admin@123");

                if (result.Succeeded)
                {
                    // Gán quyền Admin
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
            else
            {
                // Cập nhật thông tin admin nếu thay đổi
                adminUser.UserName = adminUserName;
                adminUser.FullName = adminFullName;
                adminUser.Address = adminAddress;
                await userManager.UpdateAsync(adminUser);

                // Đảm bảo vẫn giữ quyền Admin (tránh bị mất quyền)
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

                    // Tạo user customer với mật khẩu mạnh
                    var result = await userManager.CreateAsync(newCustomer, "Customer@123");

                    if (result.Succeeded)
                    {
                        // Gán quyền Customer
                        await userManager.AddToRoleAsync(newCustomer, "Customer");
                    }
                }
                else
                {
                    // Cập nhật thông tin customer nếu thay đổi
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
    }
}
