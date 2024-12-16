using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Discussions.Application.UseCases.UpdateMessage;

namespace PawsAndHearts.Discussions.IntegrationTests;

public class UpdateMessageTest : DiscussionsTestsBase
{
    private readonly ICommandHandler<UpdateMessageCommand> _sut;

    public UpdateMessageTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<UpdateMessageCommand>>();
    }

    [Fact]
    public async Task Update_Message_In_Discussion()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var discussion = await SeedDiscussionWithMessage(cancellationToken);
        var message = discussion.Messages[0];

        var command = new UpdateMessageCommand(
            discussion.Id, message.Id, discussion.Users.FirstMember, "updated");

        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        
        var foundedMessage = await _readDbContext.Messages
            .FirstOrDefaultAsync(m => m.Id == message.Id, cancellationToken);

        foundedMessage.Should().NotBeNull();
        foundedMessage?.Text.Should().Be(command.Message);
    }
}