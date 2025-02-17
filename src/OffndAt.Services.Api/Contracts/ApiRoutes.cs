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
        public const string Generate = "links";

        public const string GetByPhrase = "links/{phrase}";
    }

    /// <summary>
    ///     Contains the redirects routes.
    /// </summary>
    internal static class Redirects
    {
        public const string RedirectByPhrase = "redirects/{phrase}";
    }

    /// <summary>
    ///     Contains the languages routes.
    /// </summary>
    internal static class Languages
    {
        public const string Get = "languages";
    }

    /// <summary>
    ///     Contains the themes routes.
    /// </summary>
    internal static class Themes
    {
        public const string Get = "themes";
    }

    /// <summary>
    ///     Contains the formats routes.
    /// </summary>
    internal static class Formats
    {
        public const string Get = "formats";
    }
}
