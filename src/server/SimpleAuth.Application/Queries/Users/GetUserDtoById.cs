using SimpleAuth.Application.Abstractions.Queries;

namespace SimpleAuth.Application.Queries.Users;

public record GetUserDtoById(string Id) : IQuery<UserDto>;