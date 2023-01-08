using QueryX;
using SimpleAuth.Application.Abstractions.Queries;

namespace SimpleAuth.Application.Queries.Users;

public class ListUserDto : Query<UserDto>, IQuery<IEnumerable<UserDto>>
{
}
