using SimpleAuth.Application.Abstractions.Queries;

namespace SimpleAuth.Application.Queries.Roles;

public record GetRoleDtoById(string Id) : IQuery<RoleDto>;