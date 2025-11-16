using Finament.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Finament.Api.Persistence;


public class FinamentDbContext : DbContext
{
    public FinamentDbContext(DbContextOptions<FinamentDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });

        base.OnModelCreating(modelBuilder);
    }
}