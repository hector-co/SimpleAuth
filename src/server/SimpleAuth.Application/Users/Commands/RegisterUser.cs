using SimpleAuth.Application.Common.Commands;

namespace SimpleAuth.Application.Users.Commands;

public record RegisterUser
(
    List<string>? RolesId,
    string Email,
    string Name,
    string LastName,
    string PhoneNumber
) : ICommand<string>;
