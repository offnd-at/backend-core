using Domain.Enumerations;
using Domain.ValueObjects;


namespace OffndAt.Application.Core.Abstractions.Words;/// <summary>
///     Defines the contract for converting text between different casing styles.
/// </summary>
public interface ICaseConverter
{
    /// <summary>
    ///     Converts the specified <see cref="Word" /> instances to a specific casing and makes sure they use correct word separator.
    /// </summary>
    /// <param name="format">The casing format.</param>
    /// <param name="adverb">The adverb.</param>
    /// <param name="adjective">The adjective.</param>
    /// <param name="noun">The noun.</param>
    /// <returns>A string containing words values in a specific casing, connected with correct separator.</returns>
    string Convert(
        Format format,
        Word adverb,
        Word adjective,
        Word noun);
}
