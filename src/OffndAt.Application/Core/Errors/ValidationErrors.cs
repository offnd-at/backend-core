using OffndAt.Domain.Core.Primitives;

namespace OffndAt.Application.Core.Errors;

/// <summary>
///     Contains validation errors.
/// </summary>
internal static class ValidationErrors
{
    /// <summary>
    ///     Contains generate link errors.
    /// </summary>
    internal static class GenerateLink
    {
        internal static Error TargetUrlIsRequired => new("GenerateLink.TargetUrlIsRequired", "The target URL is required.");

        internal static Error TargetUrlMustBeAbsoluteUri =>
            new("GenerateLink.TargetUrlMustBeAbsoluteUri", "The target URL must be an absolute URI.");

        internal static Error FormatIdOutOfRange =>
            new("GenerateLink.FormatIdOutOfRange", "The format identifier must be in the range from 0 to 1 inclusive.");

        internal static Error LanguageIdOutOfRange =>
            new("GenerateLink.LanguageIdOutOfRange", "The language identifier must be in the range from 0 to 1 inclusive.");

        internal static Error ThemeIdOutOfRange =>
            new("GenerateLink.ThemeIdOutOfRange", "The theme identifier must be in the range from 0 to 2 inclusive.");
    }

    /// <summary>
    ///     Contains get link by phrase errors.
    /// </summary>
    internal static class GetLinkByPhrase
    {
        internal static Error PhraseIsRequired => new("GetLinkByPhrase.PhraseIsRequired", "The phrase is required.");
    }
}
