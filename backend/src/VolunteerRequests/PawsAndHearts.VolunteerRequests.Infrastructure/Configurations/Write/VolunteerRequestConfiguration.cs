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
            vi.OwnsOne(i => i.FullName, fb =>
            {
                fb.Property(f => f.Name)
                    .IsRequired()
                    .HasMaxLength(SharedKernel.Constants.MAX_NAME_LENGTH)
                    .HasColumnName("name");

                fb.Property(f => f.Surname)
                    .IsRequired()
                    .HasMaxLength(SharedKernel.Constants.MAX_NAME_LENGTH)
                    .HasColumnName("surname");

                fb.Property(f => f.Patronymic)
                    .IsRequired(false)
                    .HasMaxLength(SharedKernel.Constants.MAX_NAME_LENGTH)
                    .HasColumnName("patronymic");
            });

            vi.OwnsOne(i => i.Experience, eb =>
            {
                eb.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnName("experience");
            });

            vi.OwnsOne(i => i.PhoneNumber, pb =>
            {
                pb.Property(p => p.Value)
                    .IsRequired()
                    .HasMaxLength(SharedKernel.Constants.MAX_PHONE_LENGTH)
                    .HasColumnName("phone_number");
            });
            
            vi.Property(i => i.SocialNetworks)
                .HasValueObjectsJsonConversion(
                    socialNetwork => new SocialNetworkDto(socialNetwork.Name, socialNetwork.Link),
                    dto => SocialNetwork.Create(dto.Link, dto.Name).Value)
                .HasColumnName("social_networks");
        });
        
        builder.Property(v => v.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(v => v.CreatedAt)
            .HasColumnName("created_at");
        
        builder.Property(v => v.RejectionComment)
            .IsRequired(false)
            .HasMaxLength(SharedKernel.Constants.MAX_TEXT_LENGTH)
            .HasColumnName("rejection_comment");
    }
}