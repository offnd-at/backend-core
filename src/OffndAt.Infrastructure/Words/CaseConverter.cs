namespace OffndAt.Infrastructure.Words;

using Application.Core.Abstractions.Words;
using Domain.Enumerations;
using Domain.ValueObjects;
using Humanizer;

/// <summary>
///     Represents the case converter
/// </summary>
internal sealed class CaseConverter : ICaseConverter
{
    /// <inheritdoc />
    public string Convert(
        Format format,
        Word adverb,
        Word adjective,
        Word noun)
    {
        var wordsAsString = $"{adverb} {adjective} {noun}";

        if (format == Format.KebabCase)
        {
            return wordsAsString.Kebaberize();
        }

        if (format == Format.PascalCase)
        {
            return wordsAsString.Pascalize();
        }

        throw new ArgumentOutOfRangeException(nameof(format), format, "Could convert case to unknown format.");
    }
}
