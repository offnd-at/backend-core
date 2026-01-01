using FluentValidation;
using OffndAt.Application.Core.Errors;
using OffndAt.Application.Core.Extensions;

namespace OffndAt.Application.Links.Commands.GenerateLink;

/// <summary>
///     Represents the <see cref="GenerateLinkCommand" /> validator.
/// </summary>
public sealed class GenerateLinkCommandValidator : AbstractValidator<GenerateLinkCommand>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GenerateLinkCommandValidator" /> class.
    /// </summary>
    public GenerateLinkCommandValidator()
    {
        RuleFor(rq => rq.TargetUrl)
            .NotEmpty()
            .WithError(ValidationErrors.GenerateLink.TargetUrlIsRequired)
            .Must(BeAbsoluteUri)
            .WithError(ValidationErrors.GenerateLink.TargetUrlMustBeAbsoluteUri);

        RuleFor(rq => rq.FormatId).GreaterThanOrEqualTo(0).LessThanOrEqualTo(1).WithError(ValidationErrors.GenerateLink.FormatIdOutOfRange);

        RuleFor(rq => rq.LanguageId)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(1)
            .WithError(ValidationErrors.GenerateLink.LanguageIdOutOfRange);

        RuleFor(rq => rq.ThemeId).GreaterThanOrEqualTo(0).LessThanOrEqualTo(2).WithError(ValidationErrors.GenerateLink.ThemeIdOutOfRange);
    }

    private static bool BeAbsoluteUri(string url) => Uri.TryCreate(url, UriKind.Absolute, out var uri) && uri.IsAbsoluteUri;
}
