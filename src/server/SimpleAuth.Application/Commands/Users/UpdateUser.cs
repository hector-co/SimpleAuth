using System.Text.Json.Serialization;
using SimpleAuth.Application.Abstractions.Commands;

namespace SimpleAuth.Application.Commands.Users;

public record UpdateUser
(
    bool IsAdmin,
    List<int> RolesId,
    List<UpdateUser.UpdateUserClaim> Claims,
    string UserName,
    string Email,
    bool EmailConfirmed,
    string PhoneNumber,
    string DisplayName,
    string Name,
    string LastName,
    bool Disabled
) : ICommand
{
    [JsonIgnore]
    public int Id { get; set; }

    public record UpdateUserClaim(
        int Id,
        string ClaimType,
        string ClaimValue);
}