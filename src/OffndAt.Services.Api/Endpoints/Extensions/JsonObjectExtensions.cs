using System.Text.Json;
using System.Text.Json.Nodes;

namespace OffndAt.Services.Api.Endpoints.Extensions;

/// <summary>
///     Contains extension methods for JSON object manipulation.
/// </summary>
internal static class JsonObjectExtensions
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    ///     Converts the specified object to its <see cref="JsonNode" /> equivalent representation.
    /// </summary>
    /// <param name="obj">The object to convert.</param>
    /// <typeparam name="T">The object type.</typeparam>
    /// <returns>The converted object.</returns>
    /// <exception cref="ArgumentNullException">when the input object is null.</exception>
    public static JsonNode ToJsonNode<T>(this T obj) =>
        (obj == null ? throw new ArgumentNullException(nameof(obj)) : JsonSerializer.SerializeToNode(obj, Options)) ??
        throw new InvalidOperationException("Return value of JSON node serialization was null.");
}
