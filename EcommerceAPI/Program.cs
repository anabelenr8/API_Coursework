using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

// ✅ Secure Sensitive Data Using Environment Variables
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET") 
             ?? builder.Configuration["JwtSettings:SecretKey"] 
             ?? throw new InvalidOperationException("JWT_SECRET is missing!");

var dbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION") 
                         ?? builder.Configuration.GetConnectionString("DefaultConnection")
                         ?? throw new InvalidOperationException("Database connection string is missing!");

var emailPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWORD") 
                    ?? builder.Configuration["EmailSettings:SenderPassword"]
                    ?? throw new InvalidOperationException("EMAIL_PASSWORD is missing!");

// ✅ Configure Database (SQLite)
builder.Services.AddDbContext<EcommerceDbContext>(options =>
    options.UseSqlite(dbConnectionString));

// ✅ Register Services
builder.Services.AddScoped<IOrderProductService, OrderProductService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IJwtService, JwtService>(); // ✅ Refresh Token Support

// ✅ Configure Identity
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_@.";
    options.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole<int>>() // ✅ Role Support
.AddEntityFrameworkStores<EcommerceDbContext>()
.AddDefaultTokenProviders();

// ✅ Configure Authentication (JWT + Refresh Tokens)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true; // ✅ Force HTTPS
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin")); // ✅ Role-Based Access
});

// ✅ Add CORS Security Middleware
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// ✅ Add Rate Limiting (Prevent Brute Force Attacks)
builder.Services.AddRateLimiter(_ => _
    .AddFixedWindowLimiter(policyName: "Fixed", options =>
    {
        options.Window = TimeSpan.FromSeconds(10); // ✅ 10 seconds window
        options.PermitLimit = 5; // ✅ Max 5 requests per window
    })
);

// ✅ Add Swagger & JSON Formatting
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Ecommerce API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
builder.Services.AddControllers();
var app = builder.Build();

// ✅ Apply Middleware & Security Features
app.UseSwagger();
app.UseSwaggerUI(); // ✅ Ensures Swagger UI is accessible

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// ✅ Add Roles at Startup (Before Running App)
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    string[] roles = { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole<int>(role));
    }
}

app.MapControllers();
app.Run();
