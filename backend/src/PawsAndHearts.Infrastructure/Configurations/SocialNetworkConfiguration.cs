using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PawsAndHearts.Domain.Models;
using PawsAndHearts.Domain.Shared;

namespace PawsAndHearts.Infrastructure.Configurations;

public class SocialNetworkConfiguration : IEntityTypeConfiguration<SocialNetwork>
{
    public void Configure(EntityTypeBuilder<SocialNetwork> builder)
    {
        builder.ToTable("social_networks");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Link)
            .IsRequired()
            .HasMaxLength(Constants.MAX_TEXT_LENGTH);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(Constants.MAX_NAME_LENGTH);
    }
}