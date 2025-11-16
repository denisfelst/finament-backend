using Microsoft.EntityFrameworkCore;

namespace Finament.Infrastructure.Persistence;

public class FinamentDbContext : DbContext
{
    public FinamentDbContext(DbContextOptions<FinamentDbContext> options)
        : base(options)
    {
        
    }

    // Aquí luego añadimos tablas:
    // public DbSet<User> Users { get; set; }
    // public DbSet<Transaction> Transactions { get; set; }
}