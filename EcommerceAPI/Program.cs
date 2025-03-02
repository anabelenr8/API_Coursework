using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Services;
using Microsoft.OpenApi.Models;
using dotenv.net;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

// ✅ Load Environment Variables
DotEnv.Load();

// ✅ Secure Sensitive Data Using Environment Variables
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET") 
             ?? throw new InvalidOperationException("JWT_SECRET is missing!");

var dbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION") 
                         ?? builder.Configuration.GetConnectionString("DefaultConnection")
                         ?? throw new InvalidOperationException("Database connection string is missing!");

var emailPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWORD") 
                    ?? throw new InvalidOperationException("EMAIL_PASSWORD is missing!");

// 🔍 Debugging: Print loaded secrets (Do NOT use in production)
Console.WriteLine($"🔍 JWT_SECRET Loaded: {jwtKey}");
Console.WriteLine($"🔍 EMAIL_PASSWORD Loaded: {emailPassword}");

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
.AddEntityFrameworkStores<EcommerceDbContext>()
.AddDefaultTokenProviders();

// ✅ Configure Authentication (JWT Only)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"] ?? "DEFAULT_SECRET_KEY")
            )
        };
    });

// ✅ Add Middleware & Security Features
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddRateLimiter(_ => _
    .AddFixedWindowLimiter(policyName: "Fixed", options =>
    {
        options.Window = TimeSpan.FromSeconds(10);
        options.PermitLimit = 5;
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

// ✅ Apply Middleware
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseRateLimiter();
app.UseAuthentication();

app.MapControllers();
app.Run();
