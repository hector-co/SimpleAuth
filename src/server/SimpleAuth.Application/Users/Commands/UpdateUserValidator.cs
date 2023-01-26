using FluentValidation;

namespace SimpleAuth.Application.Users.Commands;

public class UpdateUserValidator : AbstractValidator<UpdateUser>
{
    public UpdateUserValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(256);
        RuleFor(c => c.LastName)
            .NotEmpty()
            .MaximumLength(256);
    }
}