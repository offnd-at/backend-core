using FluentValidation;
using OffndAt.Application.Core.Errors;
using OffndAt.Application.Core.Extensions;

namespace OffndAt.Application.Links.Queries.GetLinkByPhrase;

/// <summary>
///     Represents the <see cref="GetLinkByPhraseQuery" /> validator.
/// </summary>
public sealed class GetLinkByPhraseQueryValidator : AbstractValidator<GetLinkByPhraseQuery>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GetLinkByPhraseQueryValidator" /> class.
    /// </summary>
    public GetLinkByPhraseQueryValidator() =>
        RuleFor(rq => rq.Phrase).NotEmpty().WithError(ValidationErrors.GetLinkByPhrase.PhraseIsRequired);
}
