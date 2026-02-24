using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using Hypesoft.Domain.Entities;

namespace Hypesoft.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; init; }
    public DbSet<Category> Categories { get; init; }

    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<Product>().ToCollection("products");
        modelBuilder.Entity<Category>().ToCollection("categories");
    }
}