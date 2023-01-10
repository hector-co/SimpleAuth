namespace SimpleAuth.Application.Queries.Roles;

public record RoleClaimDto(
    string Id,
    string ClaimType,
    string ClaimValue);
