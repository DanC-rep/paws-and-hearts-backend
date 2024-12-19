using Microsoft.AspNetCore.Mvc;
using PawsAndHearts.BreedManagement.Application.Queries.GetBreedsBySpecies;
using PawsAndHearts.BreedManagement.Application.Queries.GetSpeciesWithPagination;
using PawsAndHearts.BreedManagement.Application.UseCases.CreateBreed;
using PawsAndHearts.BreedManagement.Application.UseCases.CreateSpecies;
using PawsAndHearts.BreedManagement.Application.UseCases.DeleteBreed;
using PawsAndHearts.BreedManagement.Application.UseCases.DeleteSpecies;
using PawsAndHearts.BreedManagement.Contracts.Requests;
using PawsAndHearts.Framework;
using PawsAndHearts.Framework.Authorization;
using PawsAndHearts.Framework.Extensions;

namespace PawsAndHearts.BreedManagement.Presentation;

public class SpeciesController : ApplicationController
{
    [Permission("species.create")]
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromServices] CreateSpeciesHandler handler,
        [FromBody] CreateSpeciesRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = CreateSpeciesCommand.Create(request);

        var result = await handler.Handle(command, cancellationToken);

        return result.ToResponse();
    }

    [Permission("breed.create")]
    [HttpPost("{id:guid}/breed")]
    public async Task<ActionResult<Guid>> CreateBreed(
        [FromRoute] Guid id,
        [FromBody] CreateBreedRequest request,
        [FromServices] CreateBreedHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = CreateBreedCommand.Create(id, request);
        
        var result = await handler.Handle(command, cancellationToken);

        return result.ToResponse();
    }

    [Permission("species.delete")]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Guid>> Delete(
        [FromRoute] Guid id,
        [FromServices] DeleteSpeciesHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteSpeciesCommand(id);
        
        var result = await handler.Handle(command, cancellationToken);
        
        return result.ToResponse();
    }

    [Permission("breed.delete")]
    [HttpDelete("{speciesId:guid}/breed/{breedId:guid}")]
    public async Task<ActionResult<Guid>> DeleteBreed(
        [FromRoute] Guid speciesId,
        [FromRoute] Guid breedId,
        [FromServices] DeleteBreedHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteBreedCommand(speciesId, breedId);

        var result = await handler.Handle(command, cancellationToken);

        return result.ToResponse();
    }

    [Permission("species.read")]
    [HttpGet]
    public async Task<ActionResult> Get(
        [FromQuery] GetSpeciesWithPaginationRequest request,
        [FromServices] GetSpeciesWithPaginationHandler handler,
        CancellationToken cancellationToken = default)
    {
        var query = GetSpeciesWithPaginationQuery.Create(request);
        var response = await handler.Handle(query, cancellationToken);

        return Ok(response);
    }

    [Permission("breed.read")]
    [HttpGet("breeds")]
    public async Task<ActionResult> GetBreeds(
        [FromQuery] GetBreedsBySpeciesRequest request,
        [FromServices] GetBreedsBySpeciesHandler handler,
        CancellationToken cancellationToken = default)
    {
        var query = GetBreedsBySpeciesQuery.Create(request);

        var response = await handler.Handle(query, cancellationToken);

        return Ok(response.Value);
    }
}