using FluentValidation;

namespace SimpleAuth.Application.Users.Commands;

public class RegisterUserValidator : AbstractValidator<RegisterUser>
{
    public RegisterUserValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty()
            .MaximumLength(256);
        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(256);
        RuleFor(c => c.LastName)
            .NotEmpty()
            .MaximumLength(256);
    }
}