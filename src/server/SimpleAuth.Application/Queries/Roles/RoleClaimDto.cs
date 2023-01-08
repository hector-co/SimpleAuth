namespace SimpleAuth.Application.Queries.Roles;

public record RoleClaimDto(
    int Id,
    string ClaimType,
    string ClaimValue);
