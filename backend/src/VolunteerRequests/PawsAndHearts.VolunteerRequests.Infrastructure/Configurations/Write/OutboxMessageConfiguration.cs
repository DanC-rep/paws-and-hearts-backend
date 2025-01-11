using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PawsAndHearts.VolunteerRequests.Infrastructure.Outbox;

namespace PawsAndHearts.VolunteerRequests.Infrastructure.Configurations.Write;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Payload)
            .HasColumnType("jsonb")
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(o => o.Type)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(o => o.OccuredOnUtc)
            .HasConversion(v => v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
            .IsRequired();

        builder.Property(o => o.ProcessedOnUtc)
            .HasConversion(v => v!.Value.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
            .IsRequired(false);

        builder.HasIndex(e => new
            {
                e.OccuredOnUtc,
                e.ProcessedOnUtc
            })
            .HasDatabaseName("idx_outbox_messages_unprocessed")
            .IncludeProperties(e => new
            {
                e.Id,
                e.Type,
                e.Payload
            })
            .HasFilter("processed_on_utc IS NULL");
    }
}