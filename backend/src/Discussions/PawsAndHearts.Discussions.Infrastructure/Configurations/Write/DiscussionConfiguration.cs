using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PawsAndHearts.Discussions.Domain.Entities;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;

namespace PawsAndHearts.Discussions.Infrastructure.Configurations.Write;

public class DiscussionConfiguration : IEntityTypeConfiguration<Discussion>
{
    public void Configure(EntityTypeBuilder<Discussion> builder)
    {
        builder.ToTable("discussions");
        
        builder.HasKey(d => d.Id);
        
        builder.Property(d => d.Id)
            .HasConversion(
                id => id.Value,
                value => DiscussionId.Create(value));

        builder.Property(d => d.RelationId)
            .HasColumnName("relation_id");
        
        builder.ComplexProperty(d => d.Users, du =>
        {
            du.IsRequired();
            
            du.Property(u => u.FirstMember)
                .HasColumnName("firs_member");

            du.Property(u => u.SecondMemeber)
                .HasColumnName("second_member");
        });
        
        builder.Property(d => d.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.HasMany(d => d.Messages)
            .WithOne()
            .HasForeignKey(m => m.DiscussionId);
    }
}