using FluentValidation;
using UrlShortener.Application.Dtos;

namespace UrlShortener.Application.Validators;

public class CreateUrlDtoValidator : AbstractValidator<CreateUrlDto>
{
    public CreateUrlDtoValidator()
    {
        RuleFor(x => x.OriginalUrl)
            .NotEmpty().WithMessage("Original URL is required.")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Invalid URL format.");

        RuleFor(x => x.CustomAlias)
            .MaximumLength(20).WithMessage("Custom alias cannot exceed 20 characters.")
            .Matches("^[a-zA-Z0-9-_]*$").WithMessage("Custom alias can only contain letters, numbers, hyphens and underscores.")
            .When(x => x.CustomAlias != null);

        RuleFor(x => x.ExpiresAt)
            .GreaterThan(DateTime.UtcNow).WithMessage("Expiration date must be in the future.")
            .When(x => x.ExpiresAt.HasValue);
    }
}