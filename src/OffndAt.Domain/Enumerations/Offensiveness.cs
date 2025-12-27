using OffndAt.Domain.Core.Primitives;

namespace OffndAt.Domain.Enumerations;

/// <summary>
///     Represent the offensiveness enumeration.
/// </summary>
public sealed class Offensiveness : Enumeration<Offensiveness>
{
    /// <summary>
    ///     Gets the offensive level of offensiveness.
    /// </summary>
    public static readonly Offensiveness Offensive = new(0, "offensive");

    /// <summary>
    ///     Gets the non-offensive level of offensiveness.
    /// </summary>
    public static readonly Offensiveness NonOffensive = new(1, "non-offensive");

    /// <summary>
    ///     Initializes a new instance of the <see cref="Offensiveness" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="name">The name.</param>
    private Offensiveness(int value, string name)
        : base(value, name)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Offensiveness" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <remarks>
    ///     Required by EF Core.
    /// </remarks>
    private Offensiveness(int value)
        : base(value, FromValue(value).Value.Name)
    {
    }

    /// <summary>
    ///     Creates an <see cref="Offensiveness" /> enumeration instance based on the specified boolean value.
    /// </summary>
    /// <param name="value">The boolean value.</param>
    /// <returns><see cref="Offensive" /> if provided value is true, otherwise <see cref="NonOffensive" />.</returns>
    public static Offensiveness FromBoolean(bool value) => value ? Offensive : NonOffensive;
}
