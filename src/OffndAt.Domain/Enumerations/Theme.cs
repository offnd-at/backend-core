namespace OffndAt.Domain.Enumerations;

using Core.Primitives;

/// <summary>
///     Represents the theme enumeration.
/// </summary>
public sealed class Theme : Enumeration<Theme>
{
    /// <summary>
    ///     Gets the none theme.
    /// </summary>
    public static readonly Theme None = new(0, "none");

    /// <summary>
    ///     Gets the proper names theme.
    /// </summary>
    public static readonly Theme ProperNames = new(1, "proper-names");

    /// <summary>
    ///     Gets the politicians theme.
    /// </summary>
    public static readonly Theme Politicians = new(2, "politicians");

    /// <summary>
    ///     Initializes a new instance of the <see cref="Theme" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="name">The name.</param>
    private Theme(int value, string name)
        : base(value, name)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Theme" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <remarks>
    ///     Required by EF Core.
    /// </remarks>
    private Theme(int value)
        : base(value, FromValue(value).Value.Name)
    {
    }
}
