using SimpleAuth.Application.Roles.Queries;

namespace SimpleAuth.Application.Users.Queries;

public record UserDto(
    string Id,
    List<RoleDto> Roles,
    string Email,
    bool EmailConfirmed,
    string Name,
    string LastName,
    string PhoneNumber);
