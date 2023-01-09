using SimpleAuth.Application.Abstractions.Commands;

namespace SimpleAuth.Application.Commands.Roles;

public record RegisterRole
(
    string Name,
    List<RegisterRole.RegisterRoleClaim> Claims,
    bool AssignByDefault,
    bool Disabled
) : ICommand<int>
{
    public record RegisterRoleClaim(
        string ClaimType,
        string ClaimValue);
}
