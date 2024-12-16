using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Discussions.Application.Queries.GetDiscussionByRelationId;
using PawsAndHearts.Discussions.Contracts.Dtos;

namespace PawsAndHearts.Discussions.IntegrationTests;

public class GetDiscussionByRelationIdTest : DiscussionsTestsBase
{
    private readonly IQueryHandlerWithResult<DiscussionDto, GetDiscussionByRelationIdQuery> _sut;

    public GetDiscussionByRelationIdTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<IQueryHandlerWithResult<DiscussionDto, GetDiscussionByRelationIdQuery>>();
    }

    [Fact]
    public async Task Get_Discussion_By_Relation_Id()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var discussion = await SeedDiscussion(cancellationToken);

        var query = new GetDiscussionByRelationIdQuery(discussion.RelationId);

        // act
        var result = await _sut.Handle(query, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(discussion.Id);
    }
}