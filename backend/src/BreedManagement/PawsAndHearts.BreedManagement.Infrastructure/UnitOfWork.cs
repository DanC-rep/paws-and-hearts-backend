using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using PawsAndHearts.BreedManagement.Application.Interfaces;
using PawsAndHearts.BreedManagement.Infrastructure.DbContexts;
using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.BreedManagement.Infrastructure;

public class UnitOfWork : IBreedManagementUnitOfWork
{
    private readonly WriteDbContext _dbContext;

    public UnitOfWork(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IDbTransaction> BeginTransaction(CancellationToken cancellationToken = default)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        return transaction.GetDbTransaction();
    }

    public async Task SaveChanges(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

}