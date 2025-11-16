using Finament.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Finament.Api.Persistence;


public class FinamentDbContext : DbContext
{
    public FinamentDbContext(DbContextOptions<FinamentDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }

    public DbSet<Category> Categories { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });
        
        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Color).HasColumnName("color");
            entity.Property(e => e.MonthlyLimit).HasColumnName("monthly_limit");
                            
        });

        base.OnModelCreating(modelBuilder);
    }
}