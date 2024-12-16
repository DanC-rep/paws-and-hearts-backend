using AutoFixture;
using PawsAndHearts.Discussions.Application.UseCases.CreateDiscussion;

namespace PawsAndHearts.Discussions.IntegrationTests;

public static class FixtureExtensions
{
    public static CreateDiscussionCommand CreateAddDiscussionCommand(this IFixture fixture)
    {
        return fixture.Create<CreateDiscussionCommand>();
    }
}