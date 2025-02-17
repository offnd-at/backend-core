namespace OffndAt.Services.Api.Endpoints.Extensions;

using System.Collections;
using System.Globalization;
using System.Reflection;
using Microsoft.OpenApi.Any;

/// <summary>
///     Contains extension methods for OpenAPI objects manipulation.
/// </summary>
internal static class OpenApiObjectExtensions
{
    /// <summary>
    ///     Converts the specified object to its <see cref="OpenApiObject" /> equivalent representation.
    /// </summary>
    /// <param name="obj">The object to convert.</param>
    /// <typeparam name="T">The object type.</typeparam>
    /// <returns>The converted object.</returns>
    /// <exception cref="ArgumentNullException">when input object is equal to null.</exception>
    public static OpenApiObject ToOpenApiObject<T>(this T obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        var openApiObject = new OpenApiObject();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var value = property.GetValue(obj);
            openApiObject[GetPropertyNameInCamelCase(property.Name)] = ConvertToOpenApiAny(value);
        }

        return openApiObject;
    }

    /// <summary>
    ///     Converts the specified nullable object to an <see cref="IOpenApiAny" /> instance.
    /// </summary>
    /// <param name="value">The object to convert.</param>
    /// <returns>The converted object.</returns>
    private static IOpenApiAny ConvertToOpenApiAny(object? value) =>
        value switch
        {
            null => new OpenApiNull(),
            bool b => new OpenApiBoolean(b),
            short s => new OpenApiInteger(s),
            int i => new OpenApiInteger(i),
            long l => new OpenApiLong(l),
            float f => new OpenApiFloat(f),
            decimal d => new OpenApiDouble(decimal.ToDouble(d)),
            double d => new OpenApiDouble(d),
            char c => new OpenApiString(c.ToString()),
            string s => new OpenApiString(s),
            Guid g => new OpenApiString(g.ToString()),
            DateTime d => new OpenApiDateTime(d),
            DateTimeOffset d => new OpenApiDateTime(d),
            IEnumerable e => ConvertToOpenApiArray(e),
            _ => ConvertToOpenApiObject(value)
        };

    /// <summary>
    ///     Converts the specified enumerable to an <see cref="OpenApiArray" /> instance.
    /// </summary>
    /// <param name="enumerable">The enumerable to convert.</param>
    /// <returns>The converted enumerable.</returns>
    private static OpenApiArray ConvertToOpenApiArray(IEnumerable enumerable)
    {
        var array = new OpenApiArray();
        array.AddRange(enumerable.OfType<object>().Select(ConvertToOpenApiAny));

        return array;
    }

    /// <summary>
    ///     Converts the specified object to an <see cref="OpenApiObject" /> instance.
    /// </summary>
    /// <param name="obj">The object to convert.</param>
    /// <returns>The converted object.</returns>
    private static OpenApiObject ConvertToOpenApiObject(object obj)
    {
        var openApiObject = new OpenApiObject();
        var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var value = property.GetValue(obj);
            openApiObject[GetPropertyNameInCamelCase(property.Name)] = ConvertToOpenApiAny(value);
        }

        return openApiObject;
    }

    /// <summary>
    ///     Converts the specified string to camelCase format.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <returns>The input string in camelCase format.</returns>
    private static string GetPropertyNameInCamelCase(string propertyName) =>
        char.ToLower(propertyName[0], CultureInfo.InvariantCulture) + propertyName[1..];
}
