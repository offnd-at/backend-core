using OffndAt.Application.Core.Abstractions.Messaging;
using OffndAt.Contracts.Languages;

namespace OffndAt.Application.Languages.Queries.GetSupportedLanguages;

/// <summary>
///     Represents the query used for getting supported languages.
/// </summary>
public sealed class GetSupportedLanguagesQuery : IQuery<GetSupportedLanguagesResponse>;
