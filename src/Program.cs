using Microsoft.EntityFrameworkCore;
using videogame_api.src.Models;
using Microsoft.AspNetCore.Mvc;

// Apply ApiController attribute to all Controllers
[assembly: ApiController]

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson()
    .AddXmlSerializerFormatters();

// Enable Swagger
builder.Services.AddSwaggerGen();

// Register in-memory DB
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("videogame-api"));

var app = builder.Build();

// Configure SwaggerUI endpoint at app's root
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "videogame-api");
        options.RoutePrefix = string.Empty;
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();
