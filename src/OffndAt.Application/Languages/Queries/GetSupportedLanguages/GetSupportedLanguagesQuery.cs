namespace OffndAt.Application.Languages.Queries.GetSupportedLanguages;

using Contracts.Languages;
using Core.Abstractions.Messaging;

/// <summary>
///     Query for retrieving available languages.
/// </summary>
public sealed class GetSupportedLanguagesQuery : IQuery<GetSupportedLanguagesResponse>;
