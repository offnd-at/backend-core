using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Contracts.Themes.Responses;

namespace OffndAt.Application.Themes.Queries.GetSupportedThemes;

/// <summary>
///     Represents the query used for getting supported themes.
/// </summary>
public sealed class GetSupportedThemesQuery : IQuery<GetSupportedThemesResponse>;
