using FluentAssertions;
using PawsAndHearts.Discussions.Domain.Entities;
using PawsAndHearts.Discussions.Domain.ValueObjects;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;

namespace PawsAndHearts.UnitTests;

public class DiscussionTests
{
    [Fact]
    public void Close_Discussion_That_Already_Closed()
    {
        // arrange
        var discussion = CreateDiscussion();

        // act
        discussion.CloseDiscussion();
        var result = discussion.CloseDiscussion();

        // assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Send_Message_From_User_That_Take_Part()
    {
        // arrange
        var discussion = CreateDiscussion();
        var messageId = MessageId.NewId();
        var text = "comment text";
        var message = new Message(messageId, discussion.Users.FirstMember, text, false, DateTime.UtcNow);

        // act
        var result = discussion.SendComment(message);
        
        // assert
        result.IsSuccess.Should().BeTrue();
        discussion.Messages.Should().Contain(message);
    }
    
    [Fact]
    public void Send_Message_From_User_That_Does_Not_Take_Part()
    {
        // arrange
        var discussion = CreateDiscussion();
        var messageId = MessageId.NewId();
        var text = "comment text";
        var message = new Message(messageId, Guid.NewGuid(), text, false, DateTime.UtcNow);

        // act
        var result = discussion.SendComment(message);
        
        // assert
        result.IsSuccess.Should().BeFalse();
    }
    
    [Fact]
    public void Edit_Message_By_User_That_Take_Part()
    {
        // arrange
        var discussion = CreateDiscussionWithMessage();
        var messageId = discussion.Messages.First().Id;
        var userId = discussion.Messages.First().UserId;

        // act
        var result = discussion.EditComment(userId, messageId, "comment text edited");
        
        // assert
        result.IsSuccess.Should().BeTrue();
    }
    
    [Fact]
    public void Edit_Message_By_User_That_Does_Not_Take_Part()
    {
        // arrange
        var discussion = CreateDiscussionWithMessage();
        var messageId = discussion.Messages.First().Id;

        // act
        var result = discussion.EditComment(Guid.NewGuid(), messageId, "comment text edited");
        
        // assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact] 
    public void Delete_Message_By_User_That_Take_Part()
    {
        // arrange
        var discussion = CreateDiscussionWithMessage();
        var messageId = discussion.Messages.First().Id;
        var userId = discussion.Messages.First().UserId;

        // act
        var result = discussion.DeleteComment(userId, messageId);
        
        // assert
        result.IsSuccess.Should().BeTrue();
        discussion.Messages.Should().BeEmpty();
    }
    
    [Fact] 
    public void Delete_Message_By_User_That_Does_Not_Take_Part()
    {
        // arrange
        var discussion = CreateDiscussionWithMessage();
        var messageId = discussion.Messages.First().Id;

        // act
        var result = discussion.DeleteComment(Guid.NewGuid(), messageId);
        
        // assert
        result.IsSuccess.Should().BeFalse();
    }
    
    private static Discussion CreateDiscussion()
    {
        var discussionId = DiscussionId.NewId();
        var relationId = Guid.NewGuid();
        var users = new Users(Guid.NewGuid(), Guid.NewGuid());

        return new Discussion(discussionId, users, relationId);
    }

    private static Discussion CreateDiscussionWithMessage()
    {
        var discussion = CreateDiscussion();
        var messageId = MessageId.NewId();
        var text = "comment text";

        var message = new Message(messageId, discussion.Users.FirstMember, text, false, DateTime.UtcNow);

        discussion.SendComment(message);

        return discussion;
    }
}