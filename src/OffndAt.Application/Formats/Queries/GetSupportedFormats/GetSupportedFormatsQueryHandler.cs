﻿namespace OffndAt.Application.Formats.Queries.GetSupportedFormats;

using Contracts.Formats;
using Core.Abstractions.Messaging;
using Domain.Core.Primitives;
using Domain.Enumerations;

/// <summary>
///     Represents the <see cref="GetSupportedFormatsQuery" /> handler.
/// </summary>
internal sealed class GetSupportedFormatsQueryHandler : IQueryHandler<GetSupportedFormatsQuery, GetSupportedFormatsResponse>
{
    /// <inheritdoc />
    public Task<Maybe<GetSupportedFormatsResponse>> Handle(GetSupportedFormatsQuery request, CancellationToken cancellationToken)
    {
        var formatDtos = Format.List.Select(
            format => new FormatDto
            {
                Name = format.Name,
                Value = format.Value
            });

        var maybeFormats = Maybe<GetSupportedFormatsResponse>.From(new GetSupportedFormatsResponse { Formats = formatDtos });

        return Task.FromResult(maybeFormats);
    }
}
