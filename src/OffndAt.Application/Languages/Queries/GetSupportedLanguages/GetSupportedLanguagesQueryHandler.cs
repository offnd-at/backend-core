namespace OffndAt.Application.Languages.Queries.GetSupportedLanguages;

using Contracts.Languages;
using Core.Abstractions.Messaging;
using Domain.Core.Primitives;
using Domain.Enumerations;

/// <summary>
///     Represents the <see cref="GetSupportedLanguagesQuery" /> handler.
/// </summary>
internal sealed class GetSupportedLanguagesQueryHandler : IQueryHandler<GetSupportedLanguagesQuery, GetSupportedLanguagesResponse>
{
    /// <inheritdoc />
    public Task<Maybe<GetSupportedLanguagesResponse>> Handle(GetSupportedLanguagesQuery request, CancellationToken cancellationToken)
    {
        var languageDtos = Language.List.Select(language => new LanguageDto(language.Value, language.Name, language.Code));

        var maybeLanguages = Maybe<GetSupportedLanguagesResponse>.From(new GetSupportedLanguagesResponse(languageDtos));

        return Task.FromResult(maybeLanguages);
    }
}
