using Mapster;
using Microsoft.EntityFrameworkCore;
using SimpleAuth.Domain.Model;
using SimpleAuth.Domain.Common;
using SimpleAuth.Application.Common.Commands;
using SimpleAuth.Application.Roles.Commands;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Roles.Commands;

public class UpdateRoleHandler : ICommandHandler<UpdateRole>
{
    private readonly SimpleAuthContext _context;

    public UpdateRoleHandler(SimpleAuthContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(UpdateRole request, CancellationToken cancellationToken)
    {
        var role = await _context.Set<Role>()
            .AddIncludes()
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (role == null)
            return Response.Failure(new Error("Role.Update.NotFound", "Entity not found."));

        role.Name = request.Name;
        role.AssignByDefault = request.AssignByDefault;
        role.Claims = request.Claims.Select(r => new RoleClaim
        {
            Id = r.Id,
            ClaimType = r.ClaimType,
            ClaimValue = r.ClaimValue,
        }).ToList();

        await _context.SaveChangesAsync(cancellationToken);
        return Response.Success();
    }
}