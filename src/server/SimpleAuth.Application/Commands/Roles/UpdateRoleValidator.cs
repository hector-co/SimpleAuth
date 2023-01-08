using FluentValidation;

namespace SimpleAuth.Application.Commands.Roles;

public class UpdateRoleValidator : AbstractValidator<UpdateRole>
{
    public UpdateRoleValidator()
    {
        RuleFor(c => c.Name)
            .MaximumLength(256);
        RuleForEach(c => c.Claims)
            .SetValidator(new UpdateRoleClaimValidator());
    }

    public class UpdateRoleClaimValidator : AbstractValidator<UpdateRole.UpdateRoleClaim>
    {
        public UpdateRoleClaimValidator()
        {
        }
    }

}