namespace OffndAt.Contracts.Languages;

/// <summary>
///     Represents the language data transfer object.
/// </summary>
public sealed class LanguageDto(int value, string name, string code)
{
    /// <summary>
    ///     Gets the value.
    /// </summary>
    public int Value { get; } = value;

    /// <summary>
    ///     Gets the name.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    ///     Gets the ISO 639-1 language code.
    /// </summary>
    public string Code { get; } = code;
}
