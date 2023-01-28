using SimpleAuth.Application.Common.Queries;

namespace SimpleAuth.Application.Users.Queries;

public record GetUserDtoById(Guid Id) : IQuery<UserDto>;