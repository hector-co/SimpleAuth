using Microsoft.EntityFrameworkCore;
using SimpleAuth.Domain.Model;
using SimpleAuth.Domain.Common;
using SimpleAuth.Application.Common.Commands;
using SimpleAuth.Application.Roles.Commands;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Roles.Commands;

public class DeleteRoleHandler : ICommandHandler<DeleteRole>
{
    private readonly SimpleAuthContext _context;

    public DeleteRoleHandler(SimpleAuthContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(DeleteRole request, CancellationToken cancellationToken)
    {
        var role = await _context.Set<Role>()
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (role == null)
            return Response.Failure(new Error("Role.Delete.NotFound", "Entity not found."));

        _context.Remove(role);
        await _context.SaveChangesAsync(cancellationToken);
        return Response.Success();
    }
}