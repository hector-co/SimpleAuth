using FluentValidation;

namespace SimpleAuth.Application.Roles.Commands;

public class RegisterRoleValidator : AbstractValidator<RegisterRole>
{
    public RegisterRoleValidator()
    {
        RuleFor(c => c.Name)
            .MaximumLength(256);
        RuleForEach(c => c.Claims)
            .SetValidator(new RegisterRoleClaimValidator());
    }

    public class RegisterRoleClaimValidator : AbstractValidator<RegisterRole.RegisterRoleClaim>
    {
        public RegisterRoleClaimValidator()
        {
        }
    }

}