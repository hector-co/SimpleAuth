using FluentValidation;

namespace SimpleAuth.Application.Roles.Commands;

public class UpdateRoleValidator : AbstractValidator<UpdateRole>
{
    public UpdateRoleValidator()
    {
        RuleFor(c => c.Name)
            .MaximumLength(256);
    }
}