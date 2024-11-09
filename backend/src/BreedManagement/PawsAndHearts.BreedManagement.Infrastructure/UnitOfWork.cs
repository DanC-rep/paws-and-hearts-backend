using System.Data.Common;
using Microsoft.EntityFrameworkCore.Storage;
using PawsAndHearts.BreedManagement.Infrastructure.DbContexts;
using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.BreedManagement.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly WriteDbContext _dbContext;

    public UnitOfWork(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DbTransaction> BeginTransaction(CancellationToken cancellationToken = default)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        return transaction.GetDbTransaction();
    }

    public async Task SaveChanges(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

}