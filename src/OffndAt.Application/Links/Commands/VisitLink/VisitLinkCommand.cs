using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Domain.ValueObjects;

namespace OffndAt.Application.Links.Commands.VisitLink;

/// <summary>
///     Represents the command used for visiting a link.
/// </summary>
/// <param name="phrase">The phrase used to identify the link.</param>
public sealed class VisitLinkCommand(string phrase) : ICommand<Url>
{
    /// <summary>
    ///     Gets the phrase.
    /// </summary>
    public string Phrase { get; } = phrase;
}
