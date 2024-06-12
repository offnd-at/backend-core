namespace OffndAt.Domain.Core.Errors;

using Primitives;

/// <summary>
///     Contains domain errors.
/// </summary>
public static class DomainErrors
{
    /// <summary>
    ///     Contains general errors.
    /// </summary>
    public static class General
    {
        public static Error UnprocessableRequest => new("General.UnprocessableRequest", "The server could not process the request.");

        public static Error ServerError => new("General.ServerError", "The server encountered an unrecoverable error.");
    }

    /// <summary>
    ///     Contains phrase errors.
    /// </summary>
    public static class Phrase
    {
        public static Error NullOrEmpty => new("Phrase.NullOrEmpty", "The phrase is required.");

        public static Error LongerThanAllowed => new("Phrase.LongerThanAllowed", "The phrase is longer than allowed.");

        public static Error AlreadyInUse => new("Phrase.AlreadyInUse", "The phrase is already in use.");
    }

    /// <summary>
    ///     Contains target URL errors.
    /// </summary>
    public static class Url
    {
        public static Error NullOrEmpty => new("TargetUrl.NullOrEmpty", "The target URL is required.");

        public static Error LongerThanAllowed => new("TargetUrl.LongerThanAllowed", "The target URL is longer than allowed.");
    }

    /// <summary>
    ///     Contains word errors.
    /// </summary>
    public static class Word
    {
        public static Error NullOrEmpty => new("Word.NullOrEmpty", "The word is required.");

        public static Error LongerThanAllowed => new("Word.LongerThanAllowed", "The word is longer than allowed.");
    }

    /// <summary>
    ///     Contains format errors.
    /// </summary>
    public static class Format
    {
        public static Error NotFound => new("Format.NotFound", "The format with the specified identifier was not found.");
    }

    /// <summary>
    ///     Contains language errors.
    /// </summary>
    public static class Language
    {
        public static Error NotFound => new("Language.NotFound", "The language with the specified identifier was not found.");

        public static Error NoneAvailable => new("Language.NoneAvailable", "There are no supported languages.");
    }

    /// <summary>
    ///     Contains theme errors.
    /// </summary>
    public static class Theme
    {
        public static Error NotFound => new("Theme.NotFound", "The theme with the specified identifier was not found.");

        public static Error NoneAvailable => new("Theme.NoneAvailable", "There are no supported themes.");
    }

    /// <summary>
    ///     Contains vocabulary errors.
    /// </summary>
    public static class Vocabulary
    {
        public static Error NotFound => new("Vocabulary.NotFound", "The vocabulary with the specified parameters was not found.");

        public static Error EmptyWordsList => new("Vocabulary.EmptyWordsList", "At least one word is required in the vocabulary.");
    }

    /// <summary>
    ///     Contains link errors.
    /// </summary>
    public static class Link
    {
        public static Error NotFound => new("Link.NotFound", "The link with the specified phrase was not found.");
    }
}
