using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PawsAndHearts.Core.Dtos;
using PawsAndHearts.VolunteerRequests.Contracts.Dtos;

namespace PawsAndHearts.VolunteerRequests.Infrastructure.Configurations.Read;

public class VolunteerRequestDtoConfiguration : IEntityTypeConfiguration<VolunteerRequestDto>
{
    public void Configure(EntityTypeBuilder<VolunteerRequestDto> builder)
    {
        builder.ToTable("volunteer_requests");

        builder.HasKey(v => v.Id);
        
        builder.Property(v => v.Requisites)
            .HasConversion(
                values => JsonSerializer.Serialize(string.Empty, JsonSerializerOptions.Default),
                json => JsonSerializer.Deserialize<IEnumerable<RequisiteDto>>
                    (json, JsonSerializerOptions.Default)!);
    }
}