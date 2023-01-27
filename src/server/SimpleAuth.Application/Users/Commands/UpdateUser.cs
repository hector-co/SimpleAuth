using System.Text.Json.Serialization;
using SimpleAuth.Application.Common.Commands;

namespace SimpleAuth.Application.Users.Commands;

public record UpdateUser
(
    List<string>? RolesId,
    string Name,
    string LastName,
    string PhoneNumber
) : ICommand
{
    [JsonIgnore]
    public string Id { get; set; } = string.Empty;
}