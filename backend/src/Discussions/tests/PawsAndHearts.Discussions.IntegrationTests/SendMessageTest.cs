using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Discussions.Application.UseCases.SendMessage;

namespace PawsAndHearts.Discussions.IntegrationTests;

public class SendMessageTest : DiscussionsTestsBase
{
    private readonly ICommandHandler<Guid, SendMessageCommand> _sut;

    public SendMessageTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, SendMessageCommand>>();
    }

    [Fact]
    public async Task Send_Message_To_Discussion()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;
        
        var discussion = await SeedDiscussion(cancellationToken);

        var command = new SendMessageCommand(discussion.Id, discussion.Users.FirstMember, "test");

        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        var message = await _readDbContext.Messages
            .FirstOrDefaultAsync(m => m.Id == result.Value, cancellationToken);

        message.Should().NotBeNull();
    }
}