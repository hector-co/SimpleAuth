using SimpleAuth.Application.Common.Commands;

namespace SimpleAuth.Application.Users.Commands;

public record RegisterUser
(
    List<string> RolesId,
    List<RegisterUser.RegisterUserClaim> Claims,
    string UserName,
    string Email,
    bool EmailConfirmed,
    string PhoneNumber,
    string Name,
    string LastName
) : ICommand<string>
{
    public record RegisterUserClaim(
        string ClaimType,
        string ClaimValue);
}
