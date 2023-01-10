namespace SimpleAuth.Application.Queries.Users;

public record UserClaimDto(
    string Id,
    string ClaimType,
    string ClaimValue);
