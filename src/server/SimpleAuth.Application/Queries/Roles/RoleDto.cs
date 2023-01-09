namespace SimpleAuth.Application.Queries.Roles;

public record RoleDto(
    int Id,
    string Name,
    List<RoleClaimDto> Claims,
    bool AssignByDefault,
    bool Disabled);
