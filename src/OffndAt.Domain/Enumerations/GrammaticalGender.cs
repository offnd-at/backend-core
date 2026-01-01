using OffndAt.Domain.Core.Primitives;

namespace OffndAt.Domain.Enumerations;

/// <summary>
///     Represent the grammatical gender enumeration.
/// </summary>
public sealed class GrammaticalGender : Enumeration<GrammaticalGender>
{
    /// <summary>
    ///     Gets the none gender.
    /// </summary>
    public static readonly GrammaticalGender None = new(0, "none");

    /// <summary>
    ///     Gets the masculine gender.
    /// </summary>
    public static readonly GrammaticalGender Masculine = new(1, "masculine");

    /// <summary>
    ///     Gets the feminine gender.
    /// </summary>
    public static readonly GrammaticalGender Feminine = new(2, "feminine");

    /// <summary>
    ///     Gets the neuter gender.
    /// </summary>
    public static readonly GrammaticalGender Neuter = new(3, "neuter");

    /// <summary>
    ///     Gets the masculine personal gender.
    /// </summary>
    public static readonly GrammaticalGender MasculinePersonal = new(4, "masculine-personal");

    /// <summary>
    ///     Gets the non-masculine personal gender.
    /// </summary>
    public static readonly GrammaticalGender NonMasculinePersonal = new(5, "non-masculine-personal");

    /// <summary>
    ///     Initializes a new instance of the <see cref="GrammaticalGender" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="name">The name.</param>
    private GrammaticalGender(int value, string name)
        : base(value, name)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GrammaticalGender" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <remarks>
    ///     Required by EF Core.
    /// </remarks>
    private GrammaticalGender(int value)
        : base(value, FromValue(value).Value.Name)
    {
    }
}
