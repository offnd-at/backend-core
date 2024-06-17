namespace OffndAt.Application.Formats.Queries.GetSupportedFormats;

using Contracts.Formats;
using Core.Abstractions.Messaging;
using Domain.Core.Primitives;
using Domain.Enumerations;

internal sealed class GetSupportedFormatsQueryHandler : IQueryHandler<GetSupportedFormatsQuery, GetSupportedFormatsResponse>
{
    public Task<Maybe<GetSupportedFormatsResponse>> Handle(GetSupportedFormatsQuery request, CancellationToken cancellationToken)
    {
        var formatDtos = Format.List.Select(format => new FormatDto(format.Value, format.Name));

        var maybeFormats = Maybe<GetSupportedFormatsResponse>.From(new GetSupportedFormatsResponse(formatDtos));

        return Task.FromResult(maybeFormats);
    }
}
