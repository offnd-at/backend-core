using OffndAt.Domain.Core.Primitives;

namespace OffndAt.Domain.Enumerations;

/// <summary>
///     Defines available languages for phrase generation.
/// </summary>
public sealed class Language : Enumeration<Language>
{
    /// <summary>
    ///     Gets the English language.
    /// </summary>
    public static readonly Language English = new(0, "English", "en");

    /// <summary>
    ///     Gets the Polish language.
    /// </summary>
    public static readonly Language Polish = new(1, "Polish", "pl");

    /// <summary>
    ///     Initializes a new instance of the <see cref="Language" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="name">The name.</param>
    /// <param name="code">The ISO 639-1 language code.</param>
    private Language(int value, string name, string code)
        : base(value, name) =>
        Code = code;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Language" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <remarks>
    ///     Required by EF Core.
    /// </remarks>
    private Language(int value)
        : base(value, FromValue(value).Value.Name) =>
        Code = null!;

    /// <summary>
    ///     Gets the ISO 639-1 language code.
    /// </summary>
    public string Code { get; }
}
