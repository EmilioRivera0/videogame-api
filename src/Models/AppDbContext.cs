﻿using Microsoft.EntityFrameworkCore;
using videogame_api.src.Models;

namespace videogame_api.src.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        // Database tables/sets
        public DbSet<VideogameInstance> VideogamesSet { get; set; }
        public DbSet<Genre> GenresSet { get; set; }
        public DbSet<Platform> PlatformSet { get; set; }
        public DbSet<Stock> StockSet { get; set; }
        public DbSet<Restock> RestockSet { get; set; }
    }
}
