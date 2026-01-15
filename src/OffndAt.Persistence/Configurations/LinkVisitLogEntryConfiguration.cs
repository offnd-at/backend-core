using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OffndAt.Application.Links.Models;
using OffndAt.Domain.Entities;
using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Persistence.Configurations;

/// <summary>
///     Represents the configuration for the <see cref="LinkVisitLogEntry" />.
/// </summary>
internal sealed class LinkVisitLogEntryConfiguration : IEntityTypeConfiguration<LinkVisitLogEntry>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<LinkVisitLogEntry> builder)
    {
        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Id)
            .UseIdentityByDefaultColumn()
            .IsRequired();

        builder.Property(entity => entity.LinkId)
            .HasConversion(id => id.Value, value => new LinkId(value))
            .IsRequired();

        builder.HasOne<Link>()
            .WithMany()
            .HasForeignKey(entity => entity.LinkId);

        builder.HasIndex(entity => entity.LinkId);

        builder.Property(entity => entity.VisitedAt)
            .IsRequired();

        builder.HasIndex(entity => entity.VisitedAt);

        builder.Property(entity => entity.IpAddress)
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(entity => entity.UserAgent)
            .HasMaxLength(1024)
            .IsRequired(false);

        builder.Property(entity => entity.Referrer)
            .HasMaxLength(2048)
            .IsRequired(false);

        builder.ToTable(nameof(LinkVisitLogEntry));
    }
}
