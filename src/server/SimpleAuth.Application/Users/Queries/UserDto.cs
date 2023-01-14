using SimpleAuth.Application.Roles.Queries;

namespace SimpleAuth.Application.Users.Queries;

public record UserDto(
    string Id,
    List<RoleDto> Roles,
    List<UserClaimDto> Claims,
    string UserName,
    string Email,
    bool EmailConfirmed,
    string PhoneNumber,
    string DisplayName,
    string Name,
    string LastName);
