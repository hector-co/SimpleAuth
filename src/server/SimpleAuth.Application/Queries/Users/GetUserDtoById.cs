using SimpleAuth.Application.Abstractions.Queries;

namespace SimpleAuth.Application.Queries.Users;

public record GetUserDtoById(int Id) : IQuery<UserDto>;