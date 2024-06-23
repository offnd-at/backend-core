namespace OffndAt.Services.Api.Contracts;

/// <summary>
///     Contains the API endpoint routes.
/// </summary>
internal static class ApiRoutes
{
    /// <summary>
    ///     Contains the links routes.
    /// </summary>
    internal static class Links
    {
        public const string Generate = "v{version:apiVersion}/links/generate";

        public const string GetByPhrase = "v{version:apiVersion}/links/{phrase}";

        public const string RedirectByPhrase = "v{version:apiVersion}/links/redirect/{phrase}";
    }

    /// <summary>
    ///     Contains the languages routes.
    /// </summary>
    internal static class Languages
    {
        public const string GetSupported = "v{version:apiVersion}/languages";
    }

    /// <summary>
    ///     Contains the themes routes.
    /// </summary>
    internal static class Themes
    {
        public const string GetSupported = "v{version:apiVersion}/themes";
    }

    /// <summary>
    ///     Contains the formats routes.
    /// </summary>
    internal static class Formats
    {
        public const string GetSupported = "v{version:apiVersion}/formats";
    }
}
