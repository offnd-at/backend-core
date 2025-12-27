using Core.Primitives;


namespace OffndAt.Domain.Enumerations;/// <summary>
///     Represent the part of speech enumeration.
/// </summary>
public sealed class PartOfSpeech : Enumeration<PartOfSpeech>
{
    /// <summary>
    ///     Gets the adverb part of speech.
    /// </summary>
    public static readonly PartOfSpeech Adverb = new(0, "adverb");

    /// <summary>
    ///     Gets the adjective part of speech.
    /// </summary>
    public static readonly PartOfSpeech Adjective = new(1, "adjective");

    /// <summary>
    ///     Gets the noun part of speech.
    /// </summary>
    public static readonly PartOfSpeech Noun = new(2, "noun");

    /// <summary>
    ///     Initializes a new instance of the <see cref="PartOfSpeech" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="name">The name.</param>
    private PartOfSpeech(int value, string name)
        : base(value, name)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="PartOfSpeech" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <remarks>
    ///     Required by EF Core.
    /// </remarks>
    private PartOfSpeech(int value)
        : base(value, FromValue(value).Value.Name)
    {
    }
}
