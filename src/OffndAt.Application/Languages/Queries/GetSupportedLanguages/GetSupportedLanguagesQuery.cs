namespace OffndAt.Application.Languages.Queries.GetSupportedLanguages;

using Contracts.Languages;
using Core.Abstractions.Messaging;

/// <summary>
///     Represents the query used for getting supported languages.
/// </summary>
public sealed class GetSupportedLanguagesQuery : IQuery<GetSupportedLanguagesResponse>;
