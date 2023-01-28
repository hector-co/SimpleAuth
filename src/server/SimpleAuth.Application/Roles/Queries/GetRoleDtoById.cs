using SimpleAuth.Application.Common.Queries;

namespace SimpleAuth.Application.Roles.Queries;

public record GetRoleDtoById(Guid Id) : IQuery<RoleDto>;