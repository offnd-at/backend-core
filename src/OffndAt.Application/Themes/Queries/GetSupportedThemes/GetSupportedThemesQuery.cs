namespace OffndAt.Application.Themes.Queries.GetSupportedThemes;

using Contracts.Themes;
using Core.Abstractions.Messaging;

/// <summary>
///     Represents the query used for getting supported themes.
/// </summary>
public sealed class GetSupportedThemesQuery : IQuery<GetSupportedThemesResponse>;
