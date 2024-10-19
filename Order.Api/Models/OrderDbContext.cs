using Microsoft.EntityFrameworkCore;
using Order.Api.Models.Entities;

namespace Order.Api.Models;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions options) : base(options)
    {
    }
    
    public DbSet<Entities.Order> Orders { get; set; }
    public DbSet<Entities.OrderItem> OrderItems { get; set; }
}