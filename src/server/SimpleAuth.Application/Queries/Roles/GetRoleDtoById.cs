using SimpleAuth.Application.Abstractions.Queries;

namespace SimpleAuth.Application.Queries.Roles;

public record GetRoleDtoById(int Id) : IQuery<RoleDto>;