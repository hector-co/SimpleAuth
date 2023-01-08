using Mapster;
using Microsoft.EntityFrameworkCore;
using SimpleAuth.Domain.Model;
using SimpleAuth.Application.Abstractions.Queries;
using SimpleAuth.Application.Queries.Users;
using QueryX;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Users.Queries;

public class ListUserDtoHandler : IQueryHandler<ListUserDto, IEnumerable<UserDto>>
{
    private readonly SimpleAuthContext _context;

    public ListUserDtoHandler(SimpleAuthContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<UserDto>>> Handle(ListUserDto request, CancellationToken cancellationToken)
    {
        var queryable = _context.Set<User>()
            .AddIncludes() 
            .AsNoTracking();

        queryable = queryable.ApplyQuery(request, applyOrderingAndPaging: false);
        var totalCount = await queryable.CountAsync(cancellationToken);
        queryable = queryable.ApplyOrderingAndPaging(request);

        var data = await queryable.ToListAsync(cancellationToken);

        return new Result<IEnumerable<UserDto>>(data.Adapt<List<UserDto>>(), totalCount);
    }
}