using FluentValidation;

namespace SimpleAuth.Application.Users.Commands;

public class RegisterUserValidator : AbstractValidator<RegisterUser>
{
    public RegisterUserValidator()
    {
        RuleFor(c => c.UserName)
            .MaximumLength(256);
        RuleFor(c => c.Email)
            .MaximumLength(256);
        RuleFor(c => c.Name)
            .MaximumLength(256);
        RuleFor(c => c.LastName)
            .MaximumLength(256);
        RuleForEach(c => c.Claims)
            .SetValidator(new RegisterUserClaimValidator());
    }

    public class RegisterUserClaimValidator : AbstractValidator<RegisterUser.RegisterUserClaim>
    {
        public RegisterUserClaimValidator()
        {
        }
    }

}