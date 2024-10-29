using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PawsAndHearts.Accounts.Contracts.Dtos;

namespace PawsAndHearts.Accounts.Infrastructure.Configuration.Read;

public class RoleDtoConfiguration : IEntityTypeConfiguration<RoleDto>
{
    public void Configure(EntityTypeBuilder<RoleDto> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.Id);
    }
}