namespace OffndAt.Application.Themes.Queries.GetSupportedThemes;

using Contracts.Themes;
using Core.Abstractions.Messaging;
using Domain.Core.Primitives;
using Domain.Enumerations;

/// <summary>
///     Represents the <see cref="GetSupportedThemesQuery" /> handler.
/// </summary>
internal sealed class GetSupportedThemesQueryHandler : IQueryHandler<GetSupportedThemesQuery, GetSupportedThemesResponse>
{
    /// <inheritdoc />
    public Task<Maybe<GetSupportedThemesResponse>> Handle(GetSupportedThemesQuery request, CancellationToken cancellationToken)
    {
        var themeDtos = Theme.List.Select(theme => new ThemeDto(theme.Value, theme.Name));

        var maybeThemes = Maybe<GetSupportedThemesResponse>.From(new GetSupportedThemesResponse(themeDtos));

        return Task.FromResult(maybeThemes);
    }
}
