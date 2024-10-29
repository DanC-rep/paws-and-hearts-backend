using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PawsAndHearts.Accounts.Contracts.Dtos;
using PawsAndHearts.Core.Dtos;

namespace PawsAndHearts.Accounts.Infrastructure.Configuration.Read;

public class VolunteerAccountDtoConfiguration : IEntityTypeConfiguration<VolunteerAccountDto>
{
    public void Configure(EntityTypeBuilder<VolunteerAccountDto> builder)
    {
        builder.ToTable("volunteer_accounts");

        builder.HasKey(v => v.Id);
        
        builder.Property(v => v.Requisites)
            .HasConversion(
                values => JsonSerializer.Serialize(string.Empty, JsonSerializerOptions.Default),
                json => JsonSerializer.Deserialize<IEnumerable<RequisiteDto>>
                    (json, JsonSerializerOptions.Default)!);
    }
}