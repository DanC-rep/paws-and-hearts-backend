using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Discussions.Application.Interfaces;
using PawsAndHearts.Discussions.Domain.Entities;
using PawsAndHearts.Discussions.Domain.ValueObjects;
using PawsAndHearts.Discussions.Infrastructure.DbContexts;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;

namespace PawsAndHearts.Discussions.IntegrationTests;

public class DiscussionsTestsBase : IClassFixture<IntegrationTestsWebFactory>, IAsyncLifetime
{
    protected readonly Fixture _fixture;
    protected readonly IntegrationTestsWebFactory _factory;
    protected readonly IServiceScope _scope;
    protected readonly WriteDbContext _writeDbContext;
    protected readonly IDiscussionsReadDbContext _readDbContext;

    protected DiscussionsTestsBase(IntegrationTestsWebFactory factory)
    {
        _factory = factory;
        _fixture = new Fixture();
        _scope = factory.Services.CreateScope();
        _writeDbContext = _scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        _readDbContext = _scope.ServiceProvider.GetRequiredService<IDiscussionsReadDbContext>();
    }

    public async Task InitializeAsync()
    {
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _factory.ResetDatabaseAsync();
        
        _scope.Dispose();
    }

    public async Task<Discussion> SeedDiscussion(CancellationToken cancellationToken = default)
    {
        var users = new Users(Guid.NewGuid(), Guid.NewGuid());
        
        var discussion = new Discussion(DiscussionId.NewId(), users, Guid.NewGuid());

        await _writeDbContext.AddAsync(discussion, cancellationToken);
        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return discussion;
    }

    public async Task<Discussion> SeedDiscussionWithMessage(CancellationToken cancellationToken = default)
    {
        var discussion = await SeedDiscussion(cancellationToken);

        var message = new Message(
            MessageId.NewId(),
            discussion.Users.FirstMember,
            MessageText.Create("test").Value,
            DateTime.UtcNow);

        discussion.SendComment(message);

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return discussion;
    }
}