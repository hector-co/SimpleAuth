using SimpleAuth.Application.Queries.Roles;

namespace SimpleAuth.Application.Queries.Users;

public record UserDto(
    int Id,
    bool IsAdmin,
    List<RoleDto> Roles,
    List<UserClaimDto> Claims,
    string UserName,
    string Email,
    bool EmailConfirmed,
    string PhoneNumber,
    string DisplayName,
    string Name,
    string LastName,
    bool Disabled);
