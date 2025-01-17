﻿using System.Data.Common;
using Microsoft.EntityFrameworkCore.Storage;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.VolunteerRequests.Infrastructure.DbContexts;

namespace PawsAndHearts.VolunteerRequests.Infrastructure;

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