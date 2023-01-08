namespace SimpleAuth.Application.Queries.Users;

public record UserClaimDto(
    int Id,
    string ClaimType,
    string ClaimValue);
