namespace SimpleAuth.Application.Users.Queries;

public record UserClaimDto(
    string ClaimType,
    string ClaimValue);
