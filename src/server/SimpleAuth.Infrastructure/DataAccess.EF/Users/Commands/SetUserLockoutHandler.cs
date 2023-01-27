using Microsoft.EntityFrameworkCore;
using SimpleAuth.Application.Common.Commands;
using SimpleAuth.Application.Users.Commands;
using SimpleAuth.Domain.Common;
using SimpleAuth.Domain.Model;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Users.Commands;

public class SetUserLockoutHandler : ICommandHandler<SetUserLockout>
{
    private readonly SimpleAuthContext _context;

    public SetUserLockoutHandler(SimpleAuthContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(SetUserLockout request, CancellationToken cancellationToken)
    {
        var user = await _context.Set<User>()
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (user == null)
            return Response.Failure("User.SetUserLockout.NotFound", "Entity not found.");

        user.LockoutEnd = request.LockoutEnd;

        await _context.SaveChangesAsync(cancellationToken);

        return Response.Success();
    }
}
