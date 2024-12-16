using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.SharedKernel.ValueObjects;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;
using PawsAndHearts.VolunteerRequests.Application.Interfaces;
using PawsAndHearts.VolunteerRequests.Domain.Entities;
using PawsAndHearts.VolunteerRequests.Domain.ValueObjects;
using PawsAndHearts.VolunteerRequests.Infrastructure.DbContexts;

namespace PawsAndHearts.VolunteerRequests.IntegrationTests;

public class VolunteerRequestsTestsBase : IClassFixture<IntegrationTestsWebFactory>, IAsyncLifetime
{
    protected readonly Fixture _fixture;
    protected readonly IntegrationTestsWebFactory _factory;
    protected readonly IServiceScope _scope;
    protected readonly WriteDbContext _writeDbContext;
    protected readonly IVolunteerRequestsReadDbContext _readDbContext;

    protected VolunteerRequestsTestsBase(IntegrationTestsWebFactory factory)
    {
        _factory = factory;
        _fixture = new Fixture();
        _scope = factory.Services.CreateScope();
        _writeDbContext = _scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        _readDbContext = _scope.ServiceProvider.GetRequiredService<IVolunteerRequestsReadDbContext>();
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

    protected async Task<(Guid, Guid)> SeedVolunteerRequest(CancellationToken cancellationToken = default)
    {
        var volunteerInfo = new VolunteerInfo(
            Experience.Create(5).Value,
            new List<Requisite>());

        var volunteerRequest = VolunteerRequest.CreateRequest(
            VolunteerRequestId.NewId(),
            Guid.NewGuid(),
            volunteerInfo);

        await _writeDbContext.AddAsync(volunteerRequest, cancellationToken);
        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return (volunteerRequest.Id, volunteerRequest.UserId);
    }

    protected async Task<(Guid, Guid)> TakeVolunteerRequestForSubmit(CancellationToken cancellationToken = default)
    {
        var (requestId, userId) = await SeedVolunteerRequest(cancellationToken);
        
        var volunteerRequest = await _writeDbContext.VolunteerRequests.FirstAsync(cancellationToken);
        
        volunteerRequest.TakeRequestForSubmit(Guid.NewGuid(), Guid.NewGuid());

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return (requestId, volunteerRequest.AdminId);
    }

    protected async Task<(Guid, Guid)> SendVolunteerRequestForRevision(CancellationToken cancellationToken = default)
    {
        var (requestId, adminId) = await TakeVolunteerRequestForSubmit(cancellationToken);
        
        var volunteerRequest = await _writeDbContext.VolunteerRequests.FirstAsync(cancellationToken);

        volunteerRequest.SendForRevision(RejectionComment.Create("test").Value);

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return (requestId, volunteerRequest.UserId);
    }
}