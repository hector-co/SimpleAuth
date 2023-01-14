using QueryX;
using SimpleAuth.Application.Common.Queries;

namespace SimpleAuth.Application.Users.Queries;

public class ListUserDto : Query<UserDto>, IQuery<IEnumerable<UserDto>>
{
}
