using System.Text.Json.Serialization;
using SimpleAuth.Application.Abstractions.Commands;

namespace SimpleAuth.Application.Commands.Roles;

public record UpdateRole
(
    string Name,
    List<UpdateRole.UpdateRoleClaim> Claims,
    bool DefaultRole,
    bool Disabled
) : ICommand
{
    [JsonIgnore]
    public int Id { get; set; }

    public record UpdateRoleClaim(
        int Id,
        string ClaimType,
        string ClaimValue);
}