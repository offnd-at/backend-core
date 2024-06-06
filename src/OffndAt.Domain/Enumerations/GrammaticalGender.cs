namespace OffndAt.Domain.Enumerations;

using Core.Primitives;

/// <summary>
///     Represent the grammatical gender enumeration.
/// </summary>
public sealed class GrammaticalGender : Enumeration<GrammaticalGender>
{
    public static readonly GrammaticalGender None = new(0, "none");
    public static readonly GrammaticalGender Masculine = new(1, "masculine");
    public static readonly GrammaticalGender Feminine = new(2, "feminine");
    public static readonly GrammaticalGender Neuter = new(3, "neuter");
    public static readonly GrammaticalGender MasculinePersonal = new(4, "masculine-personal");
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
