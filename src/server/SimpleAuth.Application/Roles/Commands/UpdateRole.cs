using System.Text.Json.Serialization;
using SimpleAuth.Application.Common.Commands;

namespace SimpleAuth.Application.Roles.Commands;

public record UpdateRole
(
    string Name,
    bool AssignByDefault
) : ICommand
{
    [JsonIgnore]
    public Guid Id { get; set; } 
}