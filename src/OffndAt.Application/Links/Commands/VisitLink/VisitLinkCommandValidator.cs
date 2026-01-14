using FluentValidation;
using OffndAt.Application.Core.Errors;
using OffndAt.Application.Core.Extensions;

namespace OffndAt.Application.Links.Commands.VisitLink;

/// <summary>
///     Represents the <see cref="VisitLinkCommand" /> validator.
/// </summary>
public sealed class VisitLinkCommandValidator : AbstractValidator<VisitLinkCommand>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="VisitLinkCommandValidator" /> class.
    /// </summary>
    public VisitLinkCommandValidator() => RuleFor(rq => rq.Phrase).NotEmpty().WithError(ValidationErrors.VisitLink.PhraseIsRequired);
}
