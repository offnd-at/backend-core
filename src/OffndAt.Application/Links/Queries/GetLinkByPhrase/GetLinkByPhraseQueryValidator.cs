namespace OffndAt.Application.Links.Queries.GetLinkByPhrase;

using Core.Errors;
using Core.Extensions;
using FluentValidation;

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
