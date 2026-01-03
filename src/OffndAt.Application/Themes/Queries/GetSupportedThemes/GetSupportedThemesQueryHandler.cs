using OffndAt.Application.Core.Abstractions.Messaging;
using OffndAt.Contracts.Themes.Dtos;
using OffndAt.Contracts.Themes.Responses;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.Enumerations;

namespace OffndAt.Application.Themes.Queries.GetSupportedThemes;

/// <summary>
///     Represents the <see cref="GetSupportedThemesQuery" /> handler.
/// </summary>
internal sealed class GetSupportedThemesQueryHandler : IQueryHandler<GetSupportedThemesQuery, GetSupportedThemesResponse>
{
    /// <inheritdoc />
    public Task<Maybe<GetSupportedThemesResponse>> Handle(GetSupportedThemesQuery request, CancellationToken cancellationToken)
    {
        var themeDtos = Theme.List.Select(theme => new ThemeDto
        {
            Name = theme.Name,
            Value = theme.Value
        });

        var maybeThemes = Maybe<GetSupportedThemesResponse>.From(
            new GetSupportedThemesResponse
            {
                Themes = themeDtos
            });

        return Task.FromResult(maybeThemes);
    }
}
