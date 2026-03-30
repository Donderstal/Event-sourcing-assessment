using EventSourcingAssessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventSourcingAssessment.Persistence;

public class InMemoryDbContext(DbContextOptions<InMemoryDbContext> options) : DbContext(options)
{
    public DbSet<Consumer> Consumers { get; set; }
    public DbSet<Address> Addresses { get; set; }
}