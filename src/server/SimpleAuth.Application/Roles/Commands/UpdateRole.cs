using System.Text.Json.Serialization;
using SimpleAuth.Application.Common.Commands;

namespace SimpleAuth.Application.Roles.Commands;

public record UpdateRole
(
    string Name,
    List<UpdateRole.UpdateRoleClaim> Claims,
    bool AssignByDefault
) : ICommand
{
    [JsonIgnore]
    public string Id { get; set; }

    public record UpdateRoleClaim(
        int Id,
        string ClaimType,
        string ClaimValue);
}