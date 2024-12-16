using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Discussions.Application.UseCases.CloseDiscussion;
using PawsAndHearts.Discussions.Domain.Enums;

namespace PawsAndHearts.Discussions.IntegrationTests;

public class CloseDiscussionTest : DiscussionsTestsBase
{
    private readonly ICommandHandler<CloseDiscussionCommand> _sut;

    public CloseDiscussionTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<CloseDiscussionCommand>>();
    }

    [Fact]
    public async Task Close_Discussion()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var discussion = await SeedDiscussion(cancellationToken);

        var command = new CloseDiscussionCommand(discussion.Id);

        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        var foundedDiscussion = await _readDbContext.Discussions
            .FirstOrDefaultAsync(d => d.Id == discussion.Id, cancellationToken);

        foundedDiscussion.Should().NotBeNull();
        foundedDiscussion?.Status.Should().Be(DiscussionStatus.Closed.ToString());
    }
}