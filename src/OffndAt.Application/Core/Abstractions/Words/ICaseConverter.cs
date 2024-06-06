﻿namespace OffndAt.Application.Core.Abstractions.Words;

using Domain.Enumerations;
using Domain.ValueObjects;

/// <summary>
///     Represents the case converter interface.
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
