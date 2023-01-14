using Mapster;
using Microsoft.EntityFrameworkCore;
using SimpleAuth.Domain.Model;
using QueryX;
using SimpleAuth.Application.Common.Queries;
using SimpleAuth.Application.Roles.Queries;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Roles.Queries;

public class ListRoleDtoHandler : IQueryHandler<ListRoleDto, IEnumerable<RoleDto>>
{
    private readonly SimpleAuthContext _context;

    public ListRoleDtoHandler(SimpleAuthContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<RoleDto>>> Handle(ListRoleDto request, CancellationToken cancellationToken)
    {
        var queryable = _context.Set<Role>()
            .AddIncludes() 
            .AsNoTracking();

        queryable = queryable.ApplyQuery(request, applyOrderingAndPaging: false);
        var totalCount = await queryable.CountAsync(cancellationToken);
        queryable = queryable.ApplyOrderingAndPaging(request);

        var data = await queryable.ToListAsync(cancellationToken);

        return new Result<IEnumerable<RoleDto>>(data.Adapt<List<RoleDto>>(), totalCount);
    }
}