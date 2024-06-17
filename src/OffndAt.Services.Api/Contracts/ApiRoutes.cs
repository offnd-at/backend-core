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
        public const string Generate = "links/generate";

        public const string GetByPhrase = "links/{phrase}";

        public const string RedirectByPhrase = "links/redirect/{phrase}";
    }

    /// <summary>
    ///     Contains the languages routes.
    /// </summary>
    internal static class Languages
    {
        public const string GetSupported = "languages";
    }

    /// <summary>
    ///     Contains the themes routes.
    /// </summary>
    internal static class Themes
    {
        public const string GetSupported = "themes";
    }

    /// <summary>
    ///     Contains the formats routes.
    /// </summary>
    internal static class Formats
    {
        public const string GetSupported = "formats";
    }
}
