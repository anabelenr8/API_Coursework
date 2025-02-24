using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ✅ Configure SQLite Database
builder.Services.AddDbContext<EcommerceDbContext>(options =>
    options.UseSqlite("Data Source=ecommerce.db")
);

// ✅ Configure Identity
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_@."; 
    options.User.RequireUniqueEmail = true; 
})
.AddRoles<IdentityRole<int>>() // ✅ Add Role Support
.AddEntityFrameworkStores<EcommerceDbContext>()
.AddDefaultTokenProviders();

// ✅ Configure Authentication (JWT)
var key = Encoding.UTF8.GetBytes("7S9Eg8lDJuVTkWWRpes74ALiJJV+H8yz/c/OVp/SvPI=");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();

// ✅ Add Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ✅ Make sure Swagger is **ALWAYS** available
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
