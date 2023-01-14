using QueryX;
using SimpleAuth.Application.Common.Queries;

namespace SimpleAuth.Application.Roles.Queries;

public class ListRoleDto : Query<RoleDto>, IQuery<IEnumerable<RoleDto>>
{
}
