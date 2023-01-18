using System.Text.Json.Serialization;
using SimpleAuth.Application.Common.Commands;

namespace SimpleAuth.Application.Users.Commands;

public record UpdateUser
(
    bool IsAdmin,
    List<string> RolesId,
    List<UpdateUser.UpdateUserClaim> Claims,
    string UserName,
    string Email,
    bool EmailConfirmed,
    string PhoneNumber,
    string Name,
    string LastName
) : ICommand
{
    [JsonIgnore]
    public string Id { get; set; }

    public record UpdateUserClaim(
        int Id,
        string ClaimType,
        string ClaimValue);
}