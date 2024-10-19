using Coordinator.Models;
using Microsoft.EntityFrameworkCore;

namespace Coordinator.Context;

public class TwoPhaseCommitContext : DbContext
{
    public DbSet<Node> Nodes { get; set; }
    public DbSet<NodeState> NodeStates { get; set; }

    public TwoPhaseCommitContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Node>().HasData(
            new Node("Order.Api") { Id = Guid.NewGuid() },
            new Node("Payment.Api") { Id = Guid.NewGuid() },
            new Node("Stock.Api") { Id = Guid.NewGuid() }
        );
    }
}