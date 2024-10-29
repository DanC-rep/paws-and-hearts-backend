using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PawsAndHearts.Accounts.Contracts.Dtos;
using PawsAndHearts.Core.Dtos;

namespace PawsAndHearts.Accounts.Infrastructure.Configuration.Read;

public class UserDtoConfiguration : IEntityTypeConfiguration<UserDto>
{
    public void Configure(EntityTypeBuilder<UserDto> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);
        
        builder.Property(v => v.SocialNetworks)
            .HasConversion(
                values => JsonSerializer.Serialize(string.Empty, JsonSerializerOptions.Default),
                json => JsonSerializer.Deserialize<IEnumerable<SocialNetworkDto>>
                    (json, JsonSerializerOptions.Default)!);

        builder.HasOne(u => u.AdminAccount)
            .WithOne()
            .HasForeignKey<AdminAccountDto>(a => a.UserId);

        builder.HasOne(u => u.ParticipantAccount)
            .WithOne()
            .HasForeignKey<ParticipantAccountDto>(p => p.UserId);

        builder.HasOne(u => u.VolunteerAccount)
            .WithOne()
            .HasForeignKey<VolunteerAccountDto>(v => v.UserId);
        
        builder.HasMany(u => u.Roles)
            .WithMany()
            .UsingEntity<UserRolesDto>(
                ur => ur.HasOne(ur => ur.Role).WithMany().HasForeignKey(ur => ur.RoleId),
                ur => ur.HasOne(ur => ur.User).WithMany().HasForeignKey(ur => ur.UserId)
            );
    }
}