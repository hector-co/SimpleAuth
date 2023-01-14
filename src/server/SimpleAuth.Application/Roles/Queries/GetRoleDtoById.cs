using SimpleAuth.Application.Common.Queries;

namespace SimpleAuth.Application.Roles.Queries;

public record GetRoleDtoById(string Id) : IQuery<RoleDto>;