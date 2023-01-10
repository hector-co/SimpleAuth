using SimpleAuth.Application.Queries.Roles;

namespace SimpleAuth.Application.Queries.Users;

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
