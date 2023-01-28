using System.Text.Json.Serialization;
using SimpleAuth.Application.Common.Commands;

namespace SimpleAuth.Application.Users.Commands;

public record UpdateUser
(
    List<Guid>? RolesId,
    string Name,
    string LastName,
    string PhoneNumber
) : ICommand
{
    [JsonIgnore]
    public Guid Id { get; set; } 
}