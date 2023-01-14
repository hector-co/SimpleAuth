namespace SimpleAuth.Application.Roles.Queries;

public record RoleDto(
    string Id,
    string Name,
    List<RoleClaimDto> Claims,
    bool AssignByDefault);
