using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PawsAndHearts.Accounts.Domain;
using PawsAndHearts.Core.Dtos;
using PawsAndHearts.Core.Extensions;
using PawsAndHearts.SharedKernel.ValueObjects;

namespace PawsAndHearts.Accounts.Infrastructure.Configuration.Write;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity<IdentityUserRole<Guid>>();

        builder.Property(u => u.SocialNetworks)
            .HasValueObjectsJsonConversion(
                input => new SocialNetworkDto(input.Name, input.Link),
                output => SocialNetwork.Create(output.Name, output.Link).Value)
            .HasColumnName("social_networks");
        
        builder.Property(u => u.Photo)
            .IsRequired(false)
            .HasConversion(
                photo => photo!.FileId,
                value => new Photo(value))
            .HasColumnName("photo");
        
        builder.ComplexProperty(v => v.FullName, fb =>
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
    }
}