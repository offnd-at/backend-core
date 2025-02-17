namespace OffndAt.Domain.Core.Primitives;

using System.Reflection;

/// <summary>
///     Represents an enumeration type.
/// </summary>
/// <typeparam name="TEnum">The type of the enumeration.</typeparam>
public abstract class Enumeration<TEnum> : IEquatable<Enumeration<TEnum>>, IComparable<Enumeration<TEnum>>
    where TEnum : Enumeration<TEnum>
{
    private static readonly Lazy<Dictionary<int, TEnum>> EnumerationsDictionary =
        new(() => GetAllEnumerationOptions().ToDictionary(item => item.Value));

    /// <summary>
    ///     Initializes a new instance of the <see cref="Enumeration{TEnum}" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="name">The name.</param>
    protected Enumeration(int value, string name)
    {
        Value = value;
        Name = name;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Enumeration{TEnum}" /> class.
    /// </summary>
    /// <remarks>
    ///     Required by EF Core.
    /// </remarks>
    protected Enumeration()
    {
        Value = default;
        Name = string.Empty;
    }

    /// <summary>
    ///     Gets the enumeration values.
    /// </summary>
    /// <returns>The read-only collection of enumeration values.</returns>
    public static IReadOnlyList<TEnum> List => [.. EnumerationsDictionary.Value.Values];

    /// <summary>
    ///     Gets the value.
    /// </summary>
    public int Value { get; }

    /// <summary>
    ///     Gets the name.
    /// </summary>
    public string Name { get; init; }

    /// <inheritdoc />
    public int CompareTo(Enumeration<TEnum>? other) => other is null ? 1 : Value.CompareTo(other.Value);

    /// <inheritdoc />
    public bool Equals(Enumeration<TEnum>? other)
    {
        if (other is null)
        {
            return false;
        }

        return GetType() == other.GetType() && other.Value.Equals(Value);
    }

    /// <summary>
    ///     Creates an enumeration of the specified type based on the specified value.
    /// </summary>
    /// <param name="value">The enumeration value.</param>
    /// <returns>The enumeration instance that matches the specified value.</returns>
    public static Maybe<TEnum> FromValue(int value) =>
        EnumerationsDictionary.Value.TryGetValue(value, out var enumeration)
            ? Maybe<TEnum>.From(enumeration)
            : Maybe<TEnum>.None;

    /// <summary>
    ///     Checks if the there is an enumeration with the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>True if there is an enumeration with the specified value, otherwise false.</returns>
    public static bool ContainsValue(int value) => EnumerationsDictionary.Value.ContainsKey(value);

    /// <summary>
    ///     Determines whether two <see cref="Enumeration{T}" /> instances are equal.
    /// </summary>
    /// <param name="a">The first <see cref="Enumeration{T}" /> to compare.</param>
    /// <param name="b">The second <see cref="Enumeration{T}" /> to compare.</param>
    /// <returns><c>true</c> if the two <see cref="Enumeration{T}" /> instances are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(Enumeration<TEnum>? a, Enumeration<TEnum>? b)
    {
        if (a is null && b is null)
        {
            return true;
        }

        if (a is null || b is null)
        {
            return false;
        }

        return a.Equals(b);
    }

    /// <summary>
    ///     Determines whether two <see cref="Enumeration{T}" /> instances are not equal.
    /// </summary>
    /// <param name="a">The first <see cref="Enumeration{T}" /> to compare.</param>
    /// <param name="b">The second <see cref="Enumeration{T}" /> to compare.</param>
    /// <returns><c>true</c> if the two <see cref="Enumeration{T}" /> instances are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(Enumeration<TEnum> a, Enumeration<TEnum> b) => !(a == b);

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is not Enumeration<TEnum> otherValue)
        {
            return false;
        }

        return GetType() == obj.GetType() && otherValue.Value.Equals(Value);
    }

    /// <inheritdoc />
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    ///     Gets all of the defined enumeration options.
    /// </summary>
    /// <returns>The enumerable collection of enumerations.</returns>
    private static List<TEnum> GetAllEnumerationOptions()
    {
        var enumType = typeof(TEnum);

        var enumerationTypes = Assembly
            .GetAssembly(enumType)!
            .GetTypes()
            .Where(enumType.IsAssignableFrom);

        var enumerations = new List<TEnum>();

        foreach (var enumerationType in enumerationTypes)
        {
            var enumerationTypeOptions = GetFieldsOfType<TEnum>(enumerationType);

            enumerations.AddRange(enumerationTypeOptions);
        }

        return enumerations;
    }

    /// <summary>
    ///     Gets the fields of the specified type for the specified type.
    /// </summary>
    /// <typeparam name="TFieldType">The field type.</typeparam>
    /// <param name="type">The type whose fields are being retrieved.</param>
    /// <returns>The fields of the specified type for the specified type.</returns>
    private static List<TFieldType> GetFieldsOfType<TFieldType>(Type type) =>
        type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fieldInfo => type.IsAssignableFrom(fieldInfo.FieldType))
            .Select(fieldInfo => (TFieldType?)fieldInfo.GetValue(null))
            .Where(fieldInfo => fieldInfo is not null)
            .Cast<TFieldType>()
            .ToList();

    /// <summary>
    ///     Compares two <see cref="Enumeration{T}" /> instances to determine if the left is less than the right.
    /// </summary>
    /// <param name="left">The left <see cref="Enumeration{T}" /> to compare.</param>
    /// <param name="right">The right <see cref="Enumeration{T}" /> to compare.</param>
    /// <returns><c>true</c> if the left is less than the right; otherwise, <c>false</c>.</returns>
    public static bool operator <(Enumeration<TEnum>? left, Enumeration<TEnum>? right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    /// <summary>
    ///     Compares two <see cref="Enumeration{T}" /> instances to determine if the left is less than or equal to the right.
    /// </summary>
    /// <param name="left">The left <see cref="Enumeration{T}" /> to compare.</param>
    /// <param name="right">The right <see cref="Enumeration{T}" /> to compare.</param>
    /// <returns><c>true</c> if the left is less than or equal to the right; otherwise, <c>false</c>.</returns>
    public static bool operator <=(Enumeration<TEnum>? left, Enumeration<TEnum> right) => left is null || left.CompareTo(right) <= 0;

    /// <summary>
    ///     Compares two <see cref="Enumeration{T}" /> instances to determine if the left is greater than the right.
    /// </summary>
    /// <param name="left">The left <see cref="Enumeration{T}" /> to compare.</param>
    /// <param name="right">The right <see cref="Enumeration{T}" /> to compare.</param>
    /// <returns><c>true</c> if the left is greater than the right; otherwise, <c>false</c>.</returns>
    public static bool operator >(Enumeration<TEnum>? left, Enumeration<TEnum> right) => left is not null && left.CompareTo(right) > 0;

    /// <summary>
    ///     Compares two <see cref="Enumeration{T}" /> instances to determine if the left is greater than or equal to the right.
    /// </summary>
    /// <param name="left">The left <see cref="Enumeration{T}" /> to compare.</param>
    /// <param name="right">The right <see cref="Enumeration{T}" /> to compare.</param>
    /// <returns><c>true</c> if the left is greater than or equal to the right; otherwise, <c>false</c>.</returns>
    public static bool operator >=(Enumeration<TEnum>? left, Enumeration<TEnum>? right) =>
        left is null ? right is null : left.CompareTo(right) >= 0;
}
