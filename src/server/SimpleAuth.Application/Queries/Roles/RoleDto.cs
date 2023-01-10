namespace SimpleAuth.Application.Queries.Roles;

public record RoleDto(
    string Id,
    string Name,
    List<RoleClaimDto> Claims,
    bool AssignByDefault);
