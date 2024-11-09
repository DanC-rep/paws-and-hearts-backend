using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;
using PawsAndHearts.VolunteerRequests.Domain.Entities;

namespace PawsAndHearts.VolunteerRequests.Infrastructure.Configurations.Write;

public class UserRestrictionConfiguration : IEntityTypeConfiguration<UserRestriction>
{
    public void Configure(EntityTypeBuilder<UserRestriction> builder)
    {
        builder.ToTable("user_restrictions");
        
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Id)
            .HasConversion(
                id => id.Value,
                value => UserRestrictionId.Create(value));
    }
}