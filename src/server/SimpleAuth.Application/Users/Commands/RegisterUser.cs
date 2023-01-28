using SimpleAuth.Application.Common.Commands;

namespace SimpleAuth.Application.Users.Commands;

public record RegisterUser
(
    List<Guid>? RolesId,
    string Email,
    string Name,
    string LastName,
    string? PhoneNumber
) : ICommand<Guid>;
