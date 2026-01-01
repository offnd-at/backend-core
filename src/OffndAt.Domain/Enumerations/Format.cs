using OffndAt.Domain.Core.Primitives;

namespace OffndAt.Domain.Enumerations;

/// <summary>
///     Represents the format enumeration.
/// </summary>
public sealed class Format : Enumeration<Format>
{
    /// <summary>
    ///     Gets the kebab-case format.
    /// </summary>
    public static readonly Format KebabCase = new(0, "kebab-case");

    /// <summary>
    ///     Gets the PascalCase format.
    /// </summary>
    public static readonly Format PascalCase = new(1, "pascal-case");

    /// <summary>
    ///     Initializes a new instance of the <see cref="Format" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="name">The name.</param>
    private Format(int value, string name)
        : base(value, name)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Format" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <remarks>
    ///     Required by EF Core.
    /// </remarks>
    private Format(int value)
        : base(value, FromValue(value).Value.Name)
    {
    }
}
