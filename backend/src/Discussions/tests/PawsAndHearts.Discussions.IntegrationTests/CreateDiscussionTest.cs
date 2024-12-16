using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Discussions.Application.UseCases.CreateDiscussion;

namespace PawsAndHearts.Discussions.IntegrationTests;

public class CreateDiscussionTest : DiscussionsTestsBase
{
    private readonly ICommandHandler<Guid, CreateDiscussionCommand> _sut;

    public CreateDiscussionTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, CreateDiscussionCommand>>();
    }

    [Fact]
    public async Task Add_Discussion_To_Database()
    {
        // arrange
        _factory.SetupAccountsContractMock();

        var cancellationToken = new CancellationTokenSource().Token;
        
        var command = _fixture.CreateAddDiscussionCommand();

        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        var discussion = await _readDbContext.Discussions
            .FirstOrDefaultAsync(d => d.Id == result.Value, cancellationToken);

        discussion.Should().NotBeNull();
    }
}