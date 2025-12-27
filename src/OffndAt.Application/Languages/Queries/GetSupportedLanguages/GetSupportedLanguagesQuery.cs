using OffndAt.Application.Core.Abstractions.Messaging;
using OffndAt.Contracts.Languages;

namespace OffndAt.Application.Languages.Queries.GetSupportedLanguages;

/// <summary>
///     Query for retrieving available languages.
/// </summary>
public sealed class GetSupportedLanguagesQuery : IQuery<GetSupportedLanguagesResponse>;
