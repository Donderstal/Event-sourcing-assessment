using EventSourcingAssessment.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EventSourcingAssessment.Persistence;

public class InMemoryDbContext : DbContext
{
    public DbSet<Consumer> Consumers { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public InMemoryDbContext(DbContextOptions<InMemoryDbContext> options)
        : base(options)
    {
    }

}