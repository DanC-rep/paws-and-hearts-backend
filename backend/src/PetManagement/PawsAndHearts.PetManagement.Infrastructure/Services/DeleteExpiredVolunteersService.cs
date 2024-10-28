using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PawsAndHearts.Core.Options;
using PawsAndHearts.PetManagement.Infrastructure.DbContexts;

namespace PawsAndHearts.PetManagement.Infrastructure.Services;

public class DeleteExpiredVolunteersService
{
    private readonly WriteDbContext _writeDbContext;
    private readonly SoftDeleteOptions _softDeleteOptions;

    public DeleteExpiredVolunteersService(
        WriteDbContext writeDbContext, 
        IOptions<SoftDeleteOptions> softDeleteOptions)
    {
        _writeDbContext = writeDbContext;
        _softDeleteOptions = softDeleteOptions.Value;
    }

    public async Task Process(CancellationToken cancellationToken = default)
    {
        var volunteers = await _writeDbContext.Volunteers
            .Where(v => v.IsDeleted).ToListAsync(cancellationToken);

        foreach (var volunteer in volunteers)
        {
            if (volunteer.DeletionDate!.Value.AddDays(_softDeleteOptions.ExpiredDaysTime) < DateTime.UtcNow)
                _writeDbContext.Volunteers.Remove(volunteer);
        }
        
        await _writeDbContext.SaveChangesAsync(cancellationToken);
    }
}