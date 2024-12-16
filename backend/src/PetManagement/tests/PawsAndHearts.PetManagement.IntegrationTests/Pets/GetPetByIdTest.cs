using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.PetManagement.Application.Queries.GetPetById;
using PawsAndHearts.PetManagement.Contracts.Dtos;

namespace PawsAndHearts.PetManagement.IntegrationTests.Pets;

public class GetPetByIdTest : PetManagementTestsBase
{
    private readonly IQueryHandlerWithResult<PetDto, GetPetByIdQuery> _sut;
    
    public GetPetByIdTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<IQueryHandlerWithResult<PetDto, GetPetByIdQuery>>();
    }

    [Fact]
    public async Task Get_Pet_From_Database()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var (volunteerId, petId) = await SeedPet(cancellationToken);

        var query = new GetPetByIdQuery(petId);

        // act
        var result = await _sut.Handle(query, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(petId);
    }
}