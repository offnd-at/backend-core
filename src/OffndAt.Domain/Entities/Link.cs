namespace OffndAt.Domain.Entities;

using Core.Abstractions;
using Core.Primitives;
using Core.Utils;
using Enumerations;
using Events;
using ValueObjects;
using ValueObjects.Identifiers;

/// <summary>
///     Represents the link.
/// </summary>
public sealed class Link : AggregateRoot<LinkId>, IAuditableEntity, ISoftDeletableEntity
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Link" /> class.
    /// </summary>
    /// <param name="phrase">The phrase.</param>
    /// <param name="targetUrl">The target URL.</param>
    /// <param name="language">The language.</param>
    /// <param name="theme">The theme.</param>
    private Link(
        Phrase phrase,
        Url targetUrl,
        Language language,
        Theme theme)
        : base(new LinkId(Guid.NewGuid()))
    {
        Ensure.NotEmpty(phrase, "The phrase is required.", nameof(phrase));
        Ensure.NotEmpty(targetUrl, "The target URL is required.", nameof(targetUrl));
        Ensure.NotNull(language, "The language is required.", nameof(language));
        Ensure.NotNull(theme, "The theme is required.", nameof(theme));

        Phrase = phrase;
        TargetUrl = targetUrl;
        Language = language;
        Theme = theme;
        Visits = 0;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Link" /> class.
    /// </summary>
    /// <remarks>
    ///     Required by EF Core.
    /// </remarks>
    private Link()
    {
    }

    /// <summary>
    ///     Gets the phrase.
    /// </summary>
    public Phrase Phrase { get; }

    /// <summary>
    ///     Gets the target URL.
    /// </summary>
    public Url TargetUrl { get; }

    /// <summary>
    ///     Gets the language.
    /// </summary>
    public Language Language { get; }

    /// <summary>
    ///     Gets the theme.
    /// </summary>
    public Theme Theme { get; }

    /// <summary>
    ///     Gets the visits count.
    /// </summary>
    public int Visits { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; }

    /// <inheritdoc />
    public DateTimeOffset? ModifiedAt { get; }

    /// <inheritdoc />
    public DateTimeOffset? DeletedAt { get; }

    /// <inheritdoc />
    public bool IsDeleted { get; }

    /// <summary>
    ///     Increments the visits counter.
    /// </summary>
    public void IncrementVisits()
    {
        Visits += 1;

        AddDomainEvent(new LinkVisitedDomainEvent(this));
    }

    /// <summary>
    ///     Creates a new <see cref="Link" /> with the specified arguments.
    /// </summary>
    /// <param name="phrase">The phrase.</param>
    /// <param name="targetUrl">The target URL.</param>
    /// <param name="language">The language.</param>
    /// <param name="theme">The theme.</param>
    /// <returns>The newly created <see cref="Link" /> instance.</returns>
    public static Link Create(
        Phrase phrase,
        Url targetUrl,
        Language language,
        Theme theme)
    {
        var link = new Link(
            phrase,
            targetUrl,
            language,
            theme);

        link.AddDomainEvent(new LinkCreatedDomainEvent(link));

        return link;
    }
}
