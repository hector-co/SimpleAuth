using SimpleAuth.Application.Common.Commands;

namespace SimpleAuth.Application.Roles.Commands;

public record RegisterRole
(
    string Name,
    List<RegisterRole.RegisterRoleClaim> Claims,
    bool AssignByDefault
) : ICommand<string>
{
    public record RegisterRoleClaim(
        string ClaimType,
        string ClaimValue);
}
