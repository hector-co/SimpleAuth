using Mapster;
using Microsoft.EntityFrameworkCore;
using SimpleAuth.Domain.Model;
using SimpleAuth.Application.Common.Queries;
using SimpleAuth.Application.Roles.Queries;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Roles.Queries;

public class GetRoleDtoByIdHandler : IQueryHandler<GetRoleDtoById, RoleDto>
{
    private readonly SimpleAuthContext _context;

    public GetRoleDtoByIdHandler(SimpleAuthContext context)
    {
        _context = context;
    }

    public async Task<Result<RoleDto>> Handle(GetRoleDtoById request, CancellationToken cancellationToken)
    {
        var data = await _context.Set<Role>()
            .AddIncludes()
            .AsNoTracking()
            .FirstOrDefaultAsync(m => request.Id == m.Id, cancellationToken);

        return new Result<RoleDto>(data?.Adapt<RoleDto>());
    }
}