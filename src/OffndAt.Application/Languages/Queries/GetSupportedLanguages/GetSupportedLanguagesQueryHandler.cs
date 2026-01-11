using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Contracts.Languages.Dtos;
using OffndAt.Contracts.Languages.Responses;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.Enumerations;

namespace OffndAt.Application.Languages.Queries.GetSupportedLanguages;

/// <summary>
///     Represents the <see cref="GetSupportedLanguagesQuery" /> handler.
/// </summary>
internal sealed class GetSupportedLanguagesQueryHandler : IQueryHandler<GetSupportedLanguagesQuery, GetSupportedLanguagesResponse>
{
    /// <inheritdoc />
    public Task<Maybe<GetSupportedLanguagesResponse>> Handle(GetSupportedLanguagesQuery request, CancellationToken cancellationToken)
    {
        var languageDtos = Language.List.Select(language => new LanguageDto
        {
            Code = language.Code,
            Name = language.Name,
            Value = language.Value
        });

        var maybeLanguages = Maybe<GetSupportedLanguagesResponse>.From(
            new GetSupportedLanguagesResponse
            {
                Languages = languageDtos
            });

        return Task.FromResult(maybeLanguages);
    }
}
