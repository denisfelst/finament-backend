using Finament.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Finament.Application.Infrastructure;

public interface IFinamentDbContext
{
    DbSet<Category> Categories { get; }
    DbSet<Expense> Expenses { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}