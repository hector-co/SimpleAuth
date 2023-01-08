using SimpleAuth.Application.Abstractions.Commands;

namespace SimpleAuth.Application.Commands.Users;

public record RegisterUser
(
    bool IsAdmin,
    List<int> RolesId,
    List<RegisterUser.RegisterUserClaim> Claims,
    string UserName,
    string Email,
    bool EmailConfirmed,
    string PhoneNumber,
    string DisplayName,
    string Name,
    string LastName,
    bool Disabled
) : ICommand<int>
{
    public record RegisterUserClaim(
        string ClaimType,
        string ClaimValue);
}
