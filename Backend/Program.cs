using Microsoft.EntityFrameworkCore;
using TreasureHuntApi.Data;
using TreasureHuntApi.Services;
using TreasureHuntApi.Tests;

// Check if running tests
if (args.Length > 0 && args[0] == "test")
{
    TreasureHuntServiceTests.RunAllTests();
    return;
}

if (args.Length > 0 && args[0] == "compare")
{
    var comparison = new OptimalAlgorithmTests();
    comparison.CompareAlgorithms();
    return;
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework
builder.Services.AddDbContext<TreasureHuntContext>(options =>
    options.UseInMemoryDatabase("TreasureHuntDb"));

// Add custom services
builder.Services.AddScoped<TreasureHuntService>();
builder.Services.AddScoped<OptimalTreasureHuntService>();

// Add CORS - Support both HTTP and HTTPS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000", 
                "https://localhost:3000",
                "http://localhost:3001",
                "https://localhost:3001")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Temporarily disable HTTPS redirection for easier testing
// app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TreasureHuntContext>();
    context.Database.EnsureCreated();
}

Console.WriteLine("=== Treasure Hunt API Server ===");
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
Console.WriteLine("Available URLs:");
Console.WriteLine("  - http://localhost:5000/api");
Console.WriteLine("  - https://localhost:7001/api (if HTTPS enabled)");
Console.WriteLine("  - Swagger: http://localhost:5000/swagger");
Console.WriteLine("=====================================");

app.Run(); 