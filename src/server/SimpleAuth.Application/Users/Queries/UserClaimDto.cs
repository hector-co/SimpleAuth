namespace SimpleAuth.Application.Users.Queries;

public record UserClaimDto(
    string Id,
    string ClaimType,
    string ClaimValue);
