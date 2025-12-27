namespace OffndAt.Application.Themes.Queries.GetSupportedThemes;

using Contracts.Themes;
using Core.Abstractions.Messaging;

/// <summary>
///     Query for retrieving available themes.
/// </summary>
public sealed class GetSupportedThemesQuery : IQuery<GetSupportedThemesResponse>;
