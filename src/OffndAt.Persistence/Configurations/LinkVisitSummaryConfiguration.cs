using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OffndAt.Application.Links.Models;
using OffndAt.Domain.Entities;
using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Persistence.Configurations;

/// <summary>
///     Represents the configuration for the <see cref="LinkVisitSummary" />.
/// </summary>
internal sealed class LinkVisitSummaryConfiguration : IEntityTypeConfiguration<LinkVisitSummary>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<LinkVisitSummary> builder)
    {
        builder.HasKey(entity => entity.LinkId);

        builder.Property(entity => entity.LinkId)
            .HasConversion(id => id.Value, value => new LinkId(value))
            .IsRequired();

        builder.HasOne<Link>()
            .WithOne()
            .HasForeignKey<LinkVisitSummary>(entity => entity.LinkId);

        builder.Property(entity => entity.TotalVisits)
            .IsRequired();

        builder.ToTable(nameof(LinkVisitSummary));
    }
}
