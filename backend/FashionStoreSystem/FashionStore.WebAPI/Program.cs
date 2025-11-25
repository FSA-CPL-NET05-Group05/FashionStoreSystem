

<<<<<<< HEAD
using FashionStore.Business.Interfaces.Interfaces.Admin;
﻿using FashionStore.Business.Interfaces.Interfaces.Admin;
using FashionStore.Business.Interfaces.Interfaces.Customer;
using FashionStore.Business.Interfaces.Interfaces.Login;

using FashionStore.Business.Mapping;
using FashionStore.Business.Mapping;
using FashionStore.Business.Service.Customer.Service;
using FashionStore.Business.Service.LoginService;

using FashionStore.Business.Service.Service.Admin;
using FashionStore.Business.Service.Service.Admin;
using FashionStore.Data.DBContext;
using FashionStore.Data.Interfaces.Interfaces.Admin;
using FashionStore.Data.Interfaces.Interfaces.Admin;
using FashionStore.Data.Interfaces.Interfaces.Customer;
using FashionStore.Data.Interfaces.Interfaces.Login;
using FashionStore.Data.Models;
using FashionStore.Data.Models;
using FashionStore.Data.Repositories.CustomerRepository;
using FashionStore.Data.Repositories.LoginRepository;
using FashionStore.Data.Repositories.Repositories.Admin;
using FashionStore.Data.Repositories.Repositories.Admin;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;






=======
>>>>>>> develop
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(x =>
{

    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<ILoginServices, LoginService>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();


builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Demo API",
        Version = "v1"
    });

    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(connectionString,
        b => b.MigrationsAssembly("FashionStore.Data")));

// Phải thêm Identity trước khi gọi SeedData vì SeedData cần UserManager và RoleManager
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDBContext>()
.AddDefaultTokenProviders();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SigningKey"]))
    };
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        b => b.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});



// AddScopeTuanAnh
builder.Services.AddScoped<IManagerAccountService, ManagerAccountService>();
builder.Services.AddScoped<IManagerAccountRepository, ManagerAccountRepository>();

// Product services
builder.Services.AddScoped<IAdminProductRepository, AdminProductRepository>();
builder.Services.AddScoped<IAdminProductService, AdminProductService>();

builder.Services.AddScoped<IAdminProductSizeRepository, AdminProductSizeRepository>();
builder.Services.AddScoped<IAdminProductSizeService, AdminProductSizeService>();

builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();








// Dùng Singleton vì chỉ cần tạo 1 kết nối cho toàn bộ ứng dụng
builder.Services.AddSingleton<IConnection>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var factory = new ConnectionFactory
    {
        HostName = config["RabbitMq:HostName"],
        UserName = config["RabbitMq:UserName"],
        Password = config["RabbitMq:Password"]
    };

    return factory.CreateConnectionAsync().GetAwaiter().GetResult();
});


builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IHomeRepository, HomeRepository>();

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddScoped<IRabbitMqProducer, RabbitMqProducer>();
builder.Services.AddScoped<ICustomerProductService, CustomerProductService>();
builder.Services.AddScoped<ICustomerProductRepository, CustomerProductRepository>();

builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<ICartRepository, CartRepository>();

builder.Services.AddScoped<IRabbitMqProducer, RabbitMqProducer>(); 


builder.Services.AddHostedService<OrderConsumer>();

builder.Services.AddAutoMapper(typeof(MappingProfile));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedData.Seed(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Find the error when run the seed data.");
    }
}





// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
