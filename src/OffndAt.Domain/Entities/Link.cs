using OffndAt.Domain.Core.Guards;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.Enumerations;
using OffndAt.Domain.Events;
using OffndAt.Domain.ValueObjects;
using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Domain.Entities;

/// <summary>
///     Represents the link.
/// </summary>
public sealed class Link : AggregateRoot<LinkId>
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
        Guard.AgainstEmpty(phrase, "The phrase is required.");
        Guard.AgainstEmpty(targetUrl, "The target URL is required.");
        Guard.AgainstNull(language, "The language is required.");
        Guard.AgainstNull(theme, "The theme is required.");

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
        Phrase = null!;
        TargetUrl = null!;
        Language = null!;
        Theme = null!;
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

    /// <summary>
    ///     Records a visit to the link.
    /// </summary>
    public void RecordVisit()
    {
        Visits += 1;
        RaiseDomainEvent(new LinkVisitedDomainEvent(this));
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

        link.RaiseDomainEvent(new LinkCreatedDomainEvent(link));

        return link;
    }
}
