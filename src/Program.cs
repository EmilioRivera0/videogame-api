using Microsoft.EntityFrameworkCore;
using videogame_api.src.Models;
using Microsoft.AspNetCore.Mvc;
using videogame_api.src.Formatters;

[assembly: ApiController]
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddNewtonsoftJson()
    .AddXmlSerializerFormatters();

builder.Services.AddControllers(options => options.InputFormatters.Insert(0, JPIF.GetJsonPatchInputFormatter()));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("videogame-api"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
