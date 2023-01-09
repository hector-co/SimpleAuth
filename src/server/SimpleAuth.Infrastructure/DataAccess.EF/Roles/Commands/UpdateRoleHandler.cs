using Mapster;
using Microsoft.EntityFrameworkCore;
using SimpleAuth.Domain.Model;
using SimpleAuth.Domain.Abstractions;
using SimpleAuth.Application.Abstractions.Commands;
using SimpleAuth.Application.Commands.Roles;

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
        role.Disabled = request.Disabled;
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