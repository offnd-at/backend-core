using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Contracts.Links.Responses;

namespace OffndAt.Application.Links.Commands.GenerateLink;

/// <summary>
///     Represents the command used for generating a link.
/// </summary>
public sealed class GenerateLinkCommand(
    string targetUrl,
    int languageId,
    int themeId,
    int formatId) : ICommand<GenerateLinkResponse>
{
    /// <summary>
    ///     Gets the target URL.
    /// </summary>
    public string TargetUrl { get; } = targetUrl;

    /// <summary>
    ///     Gets the language identifier.
    /// </summary>
    public int LanguageId { get; } = languageId;

    /// <summary>
    ///     Gets the theme identifier.
    /// </summary>
    public int ThemeId { get; } = themeId;

    /// <summary>
    ///     Gets the format identifier.
    /// </summary>
    public int FormatId { get; } = formatId;
}
