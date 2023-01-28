using SimpleAuth.Application.Common.Commands;
using System.Text.Json.Serialization;

namespace SimpleAuth.Application.Users.Commands;

public record SetUserLockout(DateTimeOffset? LockoutEnd) : ICommand
{
    [JsonIgnore]
    public Guid Id { get; set; }
}
