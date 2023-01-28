namespace SimpleAuth.Application.Roles.Queries;

public record RoleDto(
    Guid Id,
    string Name,
    List<RoleClaimDto> Claims,
    bool AssignByDefault);
