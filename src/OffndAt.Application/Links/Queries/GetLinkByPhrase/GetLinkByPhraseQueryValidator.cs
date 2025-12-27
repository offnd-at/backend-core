using Core.Errors;
using Core.Extensions;
using FluentValidation;


namespace OffndAt.Application.Links.Queries.GetLinkByPhrase;/// <summary>
///     Validates the GetLinkByPhraseQuery to ensure request data integrity.
/// </summary>
public sealed class GetLinkByPhraseQueryValidator : AbstractValidator<GetLinkByPhraseQuery>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GetLinkByPhraseQueryValidator" /> class.
    /// </summary>
    public GetLinkByPhraseQueryValidator() =>
        RuleFor(rq => rq.Phrase).NotEmpty().WithError(ValidationErrors.GetLinkByPhrase.PhraseIsRequired);
}
