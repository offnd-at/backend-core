using Primitives;


namespace OffndAt.Domain.Core.Errors;/// <summary>
///     Contains domain errors.
/// </summary>
public static class DomainErrors
{
    /// <summary>
    ///     Contains general errors.
    /// </summary>
    public static class General
    {
        /// <summary>
        ///     Gets the unprocessable request error.
        /// </summary>
        public static Error UnprocessableRequest => new("General.UnprocessableRequest", "The server could not process the request.");

        /// <summary>
        ///     Gets the general server error.
        /// </summary>
        public static Error ServerError => new("General.ServerError", "The server encountered an unrecoverable error.");

        /// <summary>
        ///     Gets the not found error.
        /// </summary>
        public static Error NotFound => new("General.NotFound", "The requested resource was not found.");
    }

    /// <summary>
    ///     Contains phrase errors.
    /// </summary>
    public static class Phrase
    {
        /// <summary>
        ///     Gets the null or empty error.
        /// </summary>
        public static Error NullOrEmpty => new("Phrase.NullOrEmpty", "The phrase is required.");

        /// <summary>
        ///     Gets the longer than allowed error.
        /// </summary>
        public static Error LongerThanAllowed => new("Phrase.LongerThanAllowed", "The phrase is longer than allowed.");

        /// <summary>
        ///     Gets the already in use error.
        /// </summary>
        public static Error AlreadyInUse => new("Phrase.AlreadyInUse", "The phrase is already in use.");
    }

    /// <summary>
    ///     Contains target URL errors.
    /// </summary>
    public static class Url
    {
        /// <summary>
        ///     Gets the null or empty error.
        /// </summary>
        public static Error NullOrEmpty => new("TargetUrl.NullOrEmpty", "The target URL is required.");

        /// <summary>
        ///     Gets the longer than allowed error.
        /// </summary>
        public static Error LongerThanAllowed => new("TargetUrl.LongerThanAllowed", "The target URL is longer than allowed.");
    }

    /// <summary>
    ///     Contains word errors.
    /// </summary>
    public static class Word
    {
        /// <summary>
        ///     Gets the null or empty error.
        /// </summary>
        public static Error NullOrEmpty => new("Word.NullOrEmpty", "The word is required.");

        /// <summary>
        ///     Gets the longer than allowed error.
        /// </summary>
        public static Error LongerThanAllowed => new("Word.LongerThanAllowed", "The word is longer than allowed.");
    }

    /// <summary>
    ///     Contains format errors.
    /// </summary>
    public static class Format
    {
        /// <summary>
        ///     Gets the not found error.
        /// </summary>
        public static Error NotFound => new("Format.NotFound", "The format with the specified identifier was not found.");

        /// <summary>
        ///     Gets the none available error.
        /// </summary>
        public static Error NoneAvailable => new("Format.NoneAvailable", "There are no supported formats.");
    }

    /// <summary>
    ///     Contains language errors.
    /// </summary>
    public static class Language
    {
        /// <summary>
        ///     Gets the not found error.
        /// </summary>
        public static Error NotFound => new("Language.NotFound", "The language with the specified identifier was not found.");

        /// <summary>
        ///     Gets the none available error.
        /// </summary>
        public static Error NoneAvailable => new("Language.NoneAvailable", "There are no supported languages.");
    }

    /// <summary>
    ///     Contains theme errors.
    /// </summary>
    public static class Theme
    {
        /// <summary>
        ///     Gets the not found error.
        /// </summary>
        public static Error NotFound => new("Theme.NotFound", "The theme with the specified identifier was not found.");

        /// <summary>
        ///     Gets the none available error.
        /// </summary>
        public static Error NoneAvailable => new("Theme.NoneAvailable", "There are no supported themes.");
    }

    /// <summary>
    ///     Contains vocabulary errors.
    /// </summary>
    public static class Vocabulary
    {
        /// <summary>
        ///     Gets the not found error.
        /// </summary>
        public static Error NotFound => new("Vocabulary.NotFound", "The vocabulary with the specified parameters was not found.");

        /// <summary>
        ///     Gets the empty words list error.
        /// </summary>
        public static Error EmptyWordsList => new("Vocabulary.EmptyWordsList", "At least one word is required in the vocabulary.");
    }

    /// <summary>
    ///     Contains link errors.
    /// </summary>
    public static class Link
    {
        /// <summary>
        ///     Gets the not found error.
        /// </summary>
        public static Error NotFound => new("Link.NotFound", "The link with the specified phrase was not found.");
    }
}
