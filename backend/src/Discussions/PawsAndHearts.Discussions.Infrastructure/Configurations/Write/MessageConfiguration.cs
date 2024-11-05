using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PawsAndHearts.Discussions.Domain.Entities;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;

namespace PawsAndHearts.Discussions.Infrastructure.Configurations.Write;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("messages");
        
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.Id)
            .HasConversion(
                id => id.Value,
                value => MessageId.Create(value));

        builder.Property(m => m.UserId)
            .HasColumnName("user_id");

        builder.Property(m => m.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(m => m.Text)
            .HasMaxLength(SharedKernel.Constants.MAX_TEXT_LENGTH);

        builder.Property(m => m.IsEdited)
            .HasColumnName("is_edited");
    }
}