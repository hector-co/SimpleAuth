using FluentValidation;

namespace SimpleAuth.Application.Commands.Users;

public class UpdateUserValidator : AbstractValidator<UpdateUser>
{
    public UpdateUserValidator()
    {
        RuleFor(c => c.UserName)
            .MaximumLength(256);
        RuleFor(c => c.Email)
            .MaximumLength(256);
        RuleFor(c => c.DisplayName)
            .MaximumLength(256);
        RuleFor(c => c.Name)
            .MaximumLength(256);
        RuleFor(c => c.LastName)
            .MaximumLength(256);
        RuleForEach(c => c.Claims)
            .SetValidator(new UpdateUserClaimValidator());
    }

    public class UpdateUserClaimValidator : AbstractValidator<UpdateUser.UpdateUserClaim>
    {
        public UpdateUserClaimValidator()
        {
        }
    }

}