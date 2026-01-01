using OffndAt.Application.Core.Abstractions.Messaging;
using OffndAt.Contracts.Themes;

namespace OffndAt.Application.Themes.Queries.GetSupportedThemes;

/// <summary>
///     Represents the query used for getting supported themes.
/// </summary>
public sealed class GetSupportedThemesQuery : IQuery<GetSupportedThemesResponse>;
