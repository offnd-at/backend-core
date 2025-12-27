namespace OffndAt.Application.Core.Abstractions.Data;

using Domain.Core.Primitives;

/// <summary>
///     Defines the contract for loading files from external sources.
/// </summary>
public interface IFileLoader
{
    /// <summary>
    ///     Downloads file located at the specified path.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The maybe instance that may contain the array of bytes.</returns>
    Task<Maybe<byte[]>> DownloadAsync(string filePath, CancellationToken cancellationToken = default);
}
