using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using PawsAndHearts.Accounts.Infrastructure.DbContexts;
using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.Accounts.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly AccountsWriteDbContext _writeDbContext;

    public UnitOfWork(AccountsWriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    public async Task<IDbTransaction> BeginTransaction(CancellationToken cancellationToken = default)
    {
        var transaction = await _writeDbContext.Database.BeginTransactionAsync(cancellationToken);

        return transaction.GetDbTransaction();
    }

    public async Task SaveChanges(CancellationToken cancellationToken = default)
    {
        await _writeDbContext.SaveChangesAsync(cancellationToken);
    }
}