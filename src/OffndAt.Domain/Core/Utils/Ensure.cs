namespace OffndAt.Domain.Core.Utils;

/// <summary>
///     Contains assertions for the most common application checks.
/// </summary>
public static class Ensure
{
    /// <summary>
    ///     Ensures that the specified <see cref="string" /> value is not empty.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="message">The message to show if the check fails.</param>
    /// <param name="argumentName">The name of the argument being checked.</param>
    /// <exception cref="ArgumentException"> if the specified value is empty.</exception>
    public static void NotEmpty(string value, string message, string argumentName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(message, argumentName);
        }
    }

    /// <summary>
    ///     Ensures that the specified <see cref="Guid" /> value is not empty.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="message">The message to show if the check fails.</param>
    /// <param name="argumentName">The name of the argument being checked.</param>
    /// <exception cref="ArgumentException"> if the specified value is empty.</exception>
    public static void NotEmpty(Guid value, string message, string argumentName)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(message, argumentName);
        }
    }

    /// <summary>
    ///     Ensures that the specified value is not null.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="message">The message to show if the check fails.</param>
    /// <param name="argumentName">The name of the argument being checked.</param>
    /// <exception cref="ArgumentNullException"> if the specified value is null.</exception>
    public static void NotNull<T>(T value, string message, string argumentName)
        where T : class
    {
        if (value is null)
        {
            throw new ArgumentNullException(argumentName, message);
        }
    }

    /// <summary>
    ///     Ensures that the specified <see cref="IEnumerable{T}" /> collection is not null and not empty.
    /// </summary>
    /// <param name="collection">The collection to check.</param>
    /// <param name="message">The message to show if the check fails.</param>
    /// <param name="argumentName">The name of the argument being checked.</param>
    /// <exception cref="ArgumentException"> if the specified value is empty.</exception>
    public static void NotNullAndNotEmpty<T>(IEnumerable<T> collection, string message, string argumentName)
    {
        if (collection is null || !collection.Any())
        {
            throw new ArgumentException(message, argumentName);
        }
    }
}
