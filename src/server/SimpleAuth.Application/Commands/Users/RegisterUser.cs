using SimpleAuth.Application.Abstractions.Commands;

namespace SimpleAuth.Application.Commands.Users;

public record RegisterUser
(
    List<string> RolesId,
    List<RegisterUser.RegisterUserClaim> Claims,
    string UserName,
    string Email,
    bool EmailConfirmed,
    string PhoneNumber,
    string DisplayName,
    string Name,
    string LastName
) : ICommand<string>
{
    public record RegisterUserClaim(
        string ClaimType,
        string ClaimValue);
}
