namespace OffndAt.Domain.Enumerations;

using Core.Primitives;

/// <summary>
///     Represent the grammatical number enumeration.
/// </summary>
public sealed class GrammaticalNumber : Enumeration<GrammaticalNumber>
{
    public static readonly GrammaticalNumber None = new(0, "none");
    public static readonly GrammaticalNumber Singular = new(1, "singular");
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
