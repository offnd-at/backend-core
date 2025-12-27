namespace OffndAt.Application.Core.Abstractions.Data;

using Domain.Core.Primitives;
using Domain.Models;
using Domain.ValueObjects;

/// <summary>
///     Defines the contract for loading word vocabularies from external sources.
/// </summary>
public interface IVocabularyLoader
{
    /// <summary>
    ///     Downloads vocabulary based on the specified descriptor.
    /// </summary>
    /// <param name="vocabularyDescriptor">The vocabulary descriptor.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The maybe instance that may contain the vocabulary.</returns>
    Task<Maybe<Vocabulary>> DownloadAsync(VocabularyDescriptor vocabularyDescriptor, CancellationToken cancellationToken = default);
}
