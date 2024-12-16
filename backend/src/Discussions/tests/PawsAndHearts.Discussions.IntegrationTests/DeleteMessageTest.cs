using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Discussions.Application.UseCases.DeleteMessage;

namespace PawsAndHearts.Discussions.IntegrationTests;

public class DeleteMessageTest : DiscussionsTestsBase
{
    private readonly ICommandHandler<DeleteMessageCommand> _sut;

    public DeleteMessageTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<DeleteMessageCommand>>();
    }

    [Fact]
    public async Task Delete_Message_From_Discussion()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;
        
        var discussion = await SeedDiscussionWithMessage(cancellationToken);
        var message = discussion.Messages[0];

        var command = new DeleteMessageCommand(discussion.Id, message.Id, discussion.Users.FirstMember);

        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        var messages = await _readDbContext.Messages.ToListAsync(cancellationToken);

        messages.Should().BeEmpty();
    }
}