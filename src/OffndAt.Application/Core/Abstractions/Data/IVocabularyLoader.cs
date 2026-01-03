using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.Models;
using OffndAt.Domain.ValueObjects;

namespace OffndAt.Application.Core.Abstractions.Data;

/// <summary>
///     Represents the vocabulary loader interface.
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
