using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PawsAndHearts.Discussions.Application.Interfaces;
using PawsAndHearts.Discussions.Domain.Entities;
using PawsAndHearts.Discussions.Infrastructure.DbContexts;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;

namespace PawsAndHearts.Discussions.Infrastructure.Repositories;

public class DiscussionsRepository : IDiscussionsRepository
{
    private readonly WriteDbContext _writeDbContext;

    public DiscussionsRepository(WriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }
    
    public async Task<Guid> Add(Discussion discussion, CancellationToken cancellationToken = default)
    {
        await _writeDbContext.AddAsync(discussion, cancellationToken);

        return discussion.Id;
    }

    public async Task<Result<Discussion, Error>> GetById(
        DiscussionId discussionId, 
        CancellationToken cancellationToken = default)
    {
        var discussion = await _writeDbContext.Discussions
            .Include(d => d.Messages)
            .FirstOrDefaultAsync(d => d.Id == discussionId, cancellationToken);

        if (discussion is null)
            return Errors.General.NotFound(discussionId);

        return discussion;
    }

    public Result<Guid, Error> Delete(Discussion discussion, CancellationToken cancellationToken = default)
    {
        _writeDbContext.Discussions.Remove(discussion);

        return (Guid)discussion.Id;
    }
}