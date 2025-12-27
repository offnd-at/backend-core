using Contracts.Languages;
using Core.Abstractions.Messaging;


namespace OffndAt.Application.Languages.Queries.GetSupportedLanguages;/// <summary>
///     Query for retrieving available languages.
/// </summary>
public sealed class GetSupportedLanguagesQuery : IQuery<GetSupportedLanguagesResponse>;
