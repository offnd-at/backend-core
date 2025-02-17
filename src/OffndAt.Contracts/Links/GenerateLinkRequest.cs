namespace OffndAt.Contracts.Links;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

/// <summary>
///     Represents the generate link request.
/// </summary>
public sealed class GenerateLinkRequest
{
    /// <summary>
    ///     Gets or sets the target URL.
    /// </summary>
    [Description("The target URL to which the generated link will redirect.")]
    [Required]
    public string? TargetUrl { get; init; }

    /// <summary>
    ///     Gets or sets the language identifier.
    /// </summary>
    [Description("The identifier of the desired language.")]
    [Required]
    public int? LanguageId { get; init; }

    /// <summary>
    ///     Gets or sets the theme identifier.
    /// </summary>
    [Description("The identifier of the desired theme.")]
    [Required]
    public int? ThemeId { get; init; }

    /// <summary>
    ///     Gets or sets the format identifier.
    /// </summary>
    [Description("The identifier of the desired format.")]
    [Required]
    public int? FormatId { get; init; }
}
