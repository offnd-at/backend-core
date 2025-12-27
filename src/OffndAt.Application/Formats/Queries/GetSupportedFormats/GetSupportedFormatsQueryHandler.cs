using OffndAt.Application.Core.Abstractions.Messaging;
using OffndAt.Contracts.Formats;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.Enumerations;

namespace OffndAt.Application.Formats.Queries.GetSupportedFormats;

/// <summary>
///     Handles the GetSupportedFormatsQuery to retrieve available phrase formats.
/// </summary>
internal sealed class GetSupportedFormatsQueryHandler : IQueryHandler<GetSupportedFormatsQuery, GetSupportedFormatsResponse>
{
    /// <inheritdoc />
    public Task<Maybe<GetSupportedFormatsResponse>> Handle(GetSupportedFormatsQuery request, CancellationToken cancellationToken)
    {
        var formatDtos = Format.List.Select(format => new FormatDto
        {
            Name = format.Name,
            Value = format.Value
        });

        var maybeFormats = Maybe<GetSupportedFormatsResponse>.From(new GetSupportedFormatsResponse { Formats = formatDtos });

        return Task.FromResult(maybeFormats);
    }
}
