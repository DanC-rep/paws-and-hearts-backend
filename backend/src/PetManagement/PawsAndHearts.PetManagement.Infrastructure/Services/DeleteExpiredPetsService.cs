using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PawsAndHearts.Core.Options;
using PawsAndHearts.PetManagement.Domain.Entities;
using PawsAndHearts.PetManagement.Infrastructure.DbContexts;

namespace PawsAndHearts.PetManagement.Infrastructure.Services;

public class DeleteExpiredPetsService
{
    private readonly WriteDbContext _writeDbContext;
    private readonly SoftDeleteOptions _softDeleteOptions;

    public DeleteExpiredPetsService(
        WriteDbContext writeDbContext,
        IOptions<SoftDeleteOptions> softDeleteOptions)
    {
        _writeDbContext = writeDbContext;
        _softDeleteOptions = softDeleteOptions.Value;
    }

    public async Task Process(CancellationToken cancellationToken = default)
    {
        var volunteers = await GetVolunteersWithPets(cancellationToken);

        foreach (var volunteer in volunteers)
        {
            volunteer.DeleteExpiredPets(_softDeleteOptions.ExpiredDaysTime);
        }
        
        await _writeDbContext.SaveChangesAsync(cancellationToken);
    }


    private async Task<IEnumerable<Volunteer>> GetVolunteersWithPets(
        CancellationToken cancellationToken = default)
    {
        return await _writeDbContext.Volunteers.Include(v => v.Pets).ToListAsync(cancellationToken);
    }
}