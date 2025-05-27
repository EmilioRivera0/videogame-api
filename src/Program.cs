using Microsoft.AspNetCore.Identity;
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
// Authentication Configuration
builder.Services.AddDbContext<IdentityDb>(options => options.UseInMemoryDatabase("videogame-api-identity"));
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<IdentityDb>();

var app = builder.Build();

// Authentication Configuration
app.MapIdentityApi<IdentityUser>();

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

// Populate db with test data
// Roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin" };
    foreach (var role in roles)
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
}
// Users
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    
    if (await userManager.FindByEmailAsync("admin@test.com") == null)
    {
        var adminUser = new IdentityUser()
        {
            UserName = "admin@test.com",
            Email = "admin@test.com",
        };
        var temp = await userManager.CreateAsync(adminUser, "Admin123-");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
    if (await userManager.FindByEmailAsync("user@test.com") == null)
    {
        var adminUser = new IdentityUser()
        {
            UserName = "user@test.com",
            Email = "user@test.com",
        };
        var temp = await userManager.CreateAsync(adminUser, "User123-");
    }
}

app.Run();
