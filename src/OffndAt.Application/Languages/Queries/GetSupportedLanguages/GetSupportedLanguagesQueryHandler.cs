namespace OffndAt.Application.Languages.Queries.GetSupportedLanguages;

using Contracts.Languages;
using Core.Abstractions.Messaging;
using Domain.Core.Primitives;
using Domain.Enumerations;

/// <summary>
///     Handles the GetSupportedLanguagesQuery to retrieve available languages.
/// </summary>
internal sealed class GetSupportedLanguagesQueryHandler : IQueryHandler<GetSupportedLanguagesQuery, GetSupportedLanguagesResponse>
{
    /// <inheritdoc />
    public Task<Maybe<GetSupportedLanguagesResponse>> Handle(GetSupportedLanguagesQuery request, CancellationToken cancellationToken)
    {
        var languageDtos = Language.List.Select(
            language => new LanguageDto
            {
                Code = language.Code,
                Name = language.Name,
                Value = language.Value
            });

        var maybeLanguages = Maybe<GetSupportedLanguagesResponse>.From(new GetSupportedLanguagesResponse { Languages = languageDtos });

        return Task.FromResult(maybeLanguages);
    }
}
