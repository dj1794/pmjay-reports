using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pmjay.Api.Data;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ==============================
// CORS
// ==============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});
// ==============================
// CONTROLLERS
// ==============================
builder.Services.AddControllers();
// ==============================
// DB CONTEXT
// ==============================
var conn = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrEmpty(conn))
{
    throw new InvalidOperationException(
        "Database connection string not configured. Set 'ConnectionStrings:Default'.");
}
builder.Services.AddDbContext<AgraDbContext>(options =>
    options.UseSqlServer(conn));

builder.Services.AddScoped<AgraDataService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddMemoryCache();

// ==============================
// JWT AUTHENTICATION  ??????
// ==============================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
    ),

        RoleClaimType = ClaimTypes.Role   // 🔥 FIX
    };
});
builder.Services.AddAuthorization();

// ==============================
// SWAGGER
// ==============================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<ICurrentUserService, CurrentUserService>();
var app = builder.Build();

// ==============================
// PIPELINE ORDER (IMPORTANT)
// ==============================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors("AllowOrigin");
app.UseAuthentication();   // ?? MUST COME FIRST
app.UseAuthorization();
app.MapControllers();
Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("1234"));
app.Run();