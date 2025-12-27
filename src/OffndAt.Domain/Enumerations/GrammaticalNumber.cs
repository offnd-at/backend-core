using Core.Primitives;


namespace OffndAt.Domain.Enumerations;/// <summary>
///     Represent the grammatical number enumeration.
/// </summary>
public sealed class GrammaticalNumber : Enumeration<GrammaticalNumber>
{
    /// <summary>
    ///     Gets the none number.
    /// </summary>
    public static readonly GrammaticalNumber None = new(0, "none");

    /// <summary>
    ///     Gets the singular number.
    /// </summary>
    public static readonly GrammaticalNumber Singular = new(1, "singular");

    /// <summary>
    ///     Gets the plural number.
    /// </summary>
    public static readonly GrammaticalNumber Plural = new(2, "plural");

    /// <summary>
    ///     Initializes a new instance of the <see cref="GrammaticalNumber" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="name">The name.</param>
    private GrammaticalNumber(int value, string name)
        : base(value, name)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GrammaticalNumber" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <remarks>
    ///     Required by EF Core.
    /// </remarks>
    private GrammaticalNumber(int value)
        : base(value, FromValue(value).Value.Name)
    {
    }
}
