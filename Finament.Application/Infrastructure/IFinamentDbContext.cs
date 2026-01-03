using Finament.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Finament.Application.Infrastructure;

public interface IFinamentDbContext
{
    DbSet<Category> Categories { get; }
    DbSet<Expense> Expenses { get; }
    DbSet<User> Users { get; }
    DbSet<Setting> Settings { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}