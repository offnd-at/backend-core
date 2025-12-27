namespace OffndAt.Application.Themes.Queries.GetSupportedThemes;

using Contracts.Themes;
using Core.Abstractions.Messaging;
using Domain.Core.Primitives;
using Domain.Enumerations;

/// <summary>
///     Handles the GetSupportedThemesQuery to retrieve available themes.
/// </summary>
internal sealed class GetSupportedThemesQueryHandler : IQueryHandler<GetSupportedThemesQuery, GetSupportedThemesResponse>
{
    /// <inheritdoc />
    public Task<Maybe<GetSupportedThemesResponse>> Handle(GetSupportedThemesQuery request, CancellationToken cancellationToken)
    {
        var themeDtos = Theme.List.Select(
            theme => new ThemeDto
            {
                Name = theme.Name,
                Value = theme.Value
            });

        var maybeThemes = Maybe<GetSupportedThemesResponse>.From(new GetSupportedThemesResponse { Themes = themeDtos });

        return Task.FromResult(maybeThemes);
    }
}
