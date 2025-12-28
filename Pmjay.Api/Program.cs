using Microsoft.EntityFrameworkCore;
using Pmjay.Api.Data;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<AgraDataService>();
// Configure DbContext
var conn = builder.Configuration.GetConnectionString("Default") ?? "Server=DESKTOP-PML14MH\\SQLEXPRESS;Database=GauravLab;Integrated Security=True;TrustServerCertificate=True;";
if (!string.IsNullOrEmpty(conn))
{
    builder.Services.AddDbContext<AgraDbContext>(options => options.UseSqlServer(conn));
    builder.Services.AddScoped<AgraDataService>();
}
else
{
    // Fail fast with clear message so DI doesn't produce ambiguous errors later
    throw new InvalidOperationException("Database connection string not configured. Set 'ConnectionStrings:Default' in appsettings or the 'DB_CONNECTION' environment variable.");
}
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowOrigin");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
