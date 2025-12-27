using OffndAt.Application.Core.Abstractions.Messaging;
using OffndAt.Contracts.Themes;

namespace OffndAt.Application.Themes.Queries.GetSupportedThemes;

/// <summary>
///     Query for retrieving available themes.
/// </summary>
public sealed class GetSupportedThemesQuery : IQuery<GetSupportedThemesResponse>;
