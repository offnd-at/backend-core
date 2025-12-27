using System.ComponentModel;


namespace OffndAt.Contracts.Languages;/// <summary>
///     Data transfer object for language information in API responses.
/// </summary>
public sealed class LanguageDto
{
    /// <summary>
    ///     Gets the value.
    /// </summary>
    [Description("The unique identifier of the language.")]
    public required int Value { get; init; }

    /// <summary>
    ///     Gets the name.
    /// </summary>
    [Description("The name of the language.")]
    public required string Name { get; init; }

    /// <summary>
    ///     Gets the ISO 639-1 language code.
    /// </summary>
    [Description("The ISO 639-1 language code.")]
    public required string Code { get; init; }
}
