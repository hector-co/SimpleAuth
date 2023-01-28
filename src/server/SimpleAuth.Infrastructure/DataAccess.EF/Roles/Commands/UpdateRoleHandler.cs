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
        var existentRoleId = await _context.Set<Role>().Where(r => r.NormalizedName == request.Name.ToUpper())
            .Select(r => r.Id).FirstOrDefaultAsync(cancellationToken);

        if (!(existentRoleId == request.Id))
        {
            return Response.Failure<string>("Role.Update.Duplicated", $"Role '{request.Name}' already exists");
        }

        var role = await _context.Set<Role>()
            .AddIncludes()
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (role == null)
            return Response.Failure(new Error("Role.Update.NotFound", "Entity not found."));

        role.Name = request.Name;
        role.AssignByDefault = request.AssignByDefault;

        await _context.SaveChangesAsync(cancellationToken);
        return Response.Success();
    }
}