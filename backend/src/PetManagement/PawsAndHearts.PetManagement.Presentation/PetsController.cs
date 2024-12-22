using Microsoft.AspNetCore.Mvc;
using PawsAndHearts.Core.Dtos;
using PawsAndHearts.Framework;
using PawsAndHearts.Framework.Authorization;
using PawsAndHearts.Framework.Extensions;
using PawsAndHearts.PetManagement.Application.Queries.GetPetById;
using PawsAndHearts.PetManagement.Application.Queries.GetPetsWIthPagination;
using PawsAndHearts.PetManagement.Contracts.Dtos;
using PawsAndHearts.PetManagement.Contracts.Requests.Pet;
using PawsAndHearts.PetManagement.Contracts.Responses;

namespace PawsAndHearts.PetManagement.Presentation;

public class PetsController : ApplicationController
{
    [Permission("pet.read")]
    [HttpGet]
    public async Task<ActionResult> GetPets(
        [FromQuery] GetPetsWithPaginationRequest request,
        [FromServices] GetPetsWithPaginationHandler handler,
        CancellationToken cancellationToken = default)
    {
        var query = GetPetsWithPaginationQuery.Create(request);

        var response = await handler.Handle(query, cancellationToken);

        return Ok(response);
    }

    [Permission("pet.read")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PetResponse>> GetById(
        Guid id,
        [FromServices] GetPetByIdHandler handler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPetByIdQuery(id);
        
        var result = await handler.Handle(query, cancellationToken);

        return result.ToResponse();
    }
}