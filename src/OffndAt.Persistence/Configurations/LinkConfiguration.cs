using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OffndAt.Domain.Entities;
using OffndAt.Domain.Enumerations;
using OffndAt.Domain.ValueObjects;
using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Persistence.Configurations;

/// <summary>
///     Represents the configuration for the <see cref="Link" /> entity.
/// </summary>
internal sealed class LinkConfiguration : IEntityTypeConfiguration<Link>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Link> builder)
    {
        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Id)
            .HasConversion(id => id.Value, value => new LinkId(value));

        builder.Property(entity => entity.Phrase)
            .HasConversion(phrase => phrase.Value, value => Phrase.Create(value).Value)
            .HasColumnName(nameof(Link.Phrase))
            .HasMaxLength(Phrase.MaxLength)
            .IsRequired();

        builder.HasIndex(entity => entity.Phrase);

        builder.Property(entity => entity.TargetUrl)
            .HasConversion(url => url.Value, value => Url.Create(value).Value)
            .HasColumnName(nameof(Link.TargetUrl))
            .HasMaxLength(Url.MaxLength)
            .IsRequired();

        builder.Property(entity => entity.Language)
            .HasConversion(property => property.Value, value => Language.FromValue(value))
            .IsRequired();

        builder.Property(entity => entity.Theme)
            .HasConversion(property => property.Value, value => Theme.FromValue(value))
            .IsRequired();

        builder.Property(entity => entity.Visits).IsRequired();

        builder.Property(entity => entity.CreatedAt).IsRequired();

        builder.Property(entity => entity.ModifiedAt);

        builder.Property(entity => entity.DeletedAt);

        builder.Property(entity => entity.IsDeleted).HasDefaultValue(false);

        builder.HasQueryFilter(entity => !entity.IsDeleted);
    }
}
