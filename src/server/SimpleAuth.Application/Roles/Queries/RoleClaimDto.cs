namespace SimpleAuth.Application.Roles.Queries;

public record RoleClaimDto(
    string Id,
    string ClaimType,
    string ClaimValue);
