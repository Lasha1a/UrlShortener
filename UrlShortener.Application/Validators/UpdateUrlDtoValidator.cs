using FluentValidation;
using UrlShortener.Application.Dtos;

namespace UrlShortener.Application.Validators;

public class UpdateUrlDtoValidator : AbstractValidator<UpdateUrlDto>
{
    public UpdateUrlDtoValidator()
    {
        RuleFor(x => x.OriginalUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Invalid URL format.")
            .When(x => x.OriginalUrl != null);

        RuleFor(x => x.ExpiresAt)
            .GreaterThan(DateTime.UtcNow).WithMessage("Expiration date must be in the future.")
            .When(x => x.ExpiresAt.HasValue);
    }
}