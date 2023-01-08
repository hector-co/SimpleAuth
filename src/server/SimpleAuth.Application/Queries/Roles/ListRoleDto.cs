using QueryX;
using SimpleAuth.Application.Abstractions.Queries;

namespace SimpleAuth.Application.Queries.Roles;

public class ListRoleDto : Query<RoleDto>, IQuery<IEnumerable<RoleDto>>
{
}
