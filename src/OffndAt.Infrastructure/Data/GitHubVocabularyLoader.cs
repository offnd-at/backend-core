using System.Globalization;
using System.Text;
using OffndAt.Application.Abstractions.Data;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.Models;
using OffndAt.Domain.ValueObjects;

namespace OffndAt.Infrastructure.Data;

/// <summary>
///     Represents the GitHub vocabulary loader.
/// </summary>
/// <param name="fileLoader">The file loader.</param>
internal sealed class GitHubVocabularyLoader(IFileLoader fileLoader) : IVocabularyLoader
{
    /// <inheritdoc />
    public async Task<Maybe<Vocabulary>> DownloadAsync(
        VocabularyDescriptor vocabularyDescriptor,
        CancellationToken cancellationToken = default)
    {
        var path = GetPath(vocabularyDescriptor);

        var fileContent = await fileLoader.DownloadAsync(path, cancellationToken);
        if (fileContent.HasNoValue)
        {
            return Maybe<Vocabulary>.None;
        }

        var stringContent = Encoding.UTF8.GetString(fileContent);

        var rawWords = stringContent.Split('\n');

        var words = rawWords
            .Select(Word.Create)
            .Where(result => result.IsSuccess)
            .Select(result => result.Value)
            .ToList();

        var vocabularyResult = Vocabulary.Create(vocabularyDescriptor, words);

        return vocabularyResult.IsFailure
            ? Maybe<Vocabulary>.None
            : Maybe<Vocabulary>.From(vocabularyResult.Value);
    }

    private static string GetPath(VocabularyDescriptor vocabularyDescriptor) =>
        new StringBuilder(string.Empty)
            .Append(CultureInfo.InvariantCulture, $"/{vocabularyDescriptor.Language.Code.ToLowerInvariant()}")
            .Append(CultureInfo.InvariantCulture, $"/{vocabularyDescriptor.Offensiveness.Name.ToLowerInvariant()}")
            .Append(CultureInfo.InvariantCulture, $"/{vocabularyDescriptor.GrammaticalNumber.Name.ToLowerInvariant()}")
            .Append(CultureInfo.InvariantCulture, $"/{vocabularyDescriptor.GrammaticalGender.Name.ToLowerInvariant()}")
            .Append(CultureInfo.InvariantCulture, $"/{vocabularyDescriptor.Theme.Name.ToLowerInvariant()}")
            .Append(CultureInfo.InvariantCulture, $"/{vocabularyDescriptor.PartOfSpeech.Name.ToLowerInvariant()}s.txt")
            .ToString();
}
