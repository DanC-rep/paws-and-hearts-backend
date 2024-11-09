using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PawsAndHearts.Core.Dtos;
using PawsAndHearts.Core.Extensions;
using PawsAndHearts.SharedKernel.ValueObjects;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;
using PawsAndHearts.VolunteerRequests.Domain.Entities;

namespace PawsAndHearts.VolunteerRequests.Infrastructure.Configurations.Write;

public class VolunteerRequestConfiguration : IEntityTypeConfiguration<VolunteerRequest>
{
    public void Configure(EntityTypeBuilder<VolunteerRequest> builder)
    {
        builder.ToTable("volunteer_requests");
        
        builder.HasKey(v => v.Id);
        
        builder.Property(v => v.Id)
            .HasConversion(
                id => id.Value,
                value => VolunteerRequestId.Create(value));

        builder.Property(v => v.UserId)
            .HasColumnName("user_id");
        
        builder.Property(v => v.AdminId)
            .HasColumnName("admin_id");

        builder.Property(v => v.DiscussionId)
            .HasColumnName("discussion_id");

        builder.OwnsOne(v => v.VolunteerInfo, vi =>
        {
            vi.OwnsOne(i => i.Experience, eb =>
            {
                eb.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnName("experience");
            });
            
            vi.Property(i => i.Requisites)
                .HasValueObjectsJsonConversion(
                    requisite => new RequisiteDto(requisite.Name, requisite.Description),
                    dto => Requisite.Create(dto.Name, dto.Description).Value)
                .HasColumnName("requisites");
        });
        
        builder.Property(v => v.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(v => v.CreatedAt)
            .HasColumnName("created_at");

        builder.ComplexProperty(v => v.RejectionComment, rb =>
        {
            rb.Property(c => c.Value)
                .IsRequired()
                .HasMaxLength(SharedKernel.Constants.MAX_DESCRIPTION_LENGTH)
                .HasColumnName("rejection_comment");
        });
    }
}