using Mapster;
using Microsoft.EntityFrameworkCore;
using SimpleAuth.Domain.Model;
using SimpleAuth.Application.Abstractions.Queries;
using SimpleAuth.Application.Queries.Users;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Users.Queries;

public class GetUserDtoByIdHandler : IQueryHandler<GetUserDtoById, UserDto>
{
    private readonly SimpleAuthContext _context;

    public GetUserDtoByIdHandler(SimpleAuthContext context)
    {
        _context = context;
    }

    public async Task<Result<UserDto>> Handle(GetUserDtoById request, CancellationToken cancellationToken)
    {
        var data = await _context.Set<User>()
            .AddIncludes()
            .AsNoTracking()
            .FirstOrDefaultAsync(m => request.Id == m.Id, cancellationToken);

        return new Result<UserDto>(data?.Adapt<UserDto>());
    }
}