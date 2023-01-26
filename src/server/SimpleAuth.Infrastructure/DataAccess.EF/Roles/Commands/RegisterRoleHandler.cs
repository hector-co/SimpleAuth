using Mapster;
using Microsoft.EntityFrameworkCore;
using SimpleAuth.Domain.Model;
using SimpleAuth.Domain.Common;
using SimpleAuth.Application.Common.Commands;
using SimpleAuth.Application.Roles.Commands;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Roles.Commands;

public class RegisterRoleHandler : ICommandHandler<RegisterRole, string>
{
    private readonly SimpleAuthContext _context;

    public RegisterRoleHandler(SimpleAuthContext context)
    {
        _context = context;
    }

    public async Task<Response<string>> Handle(RegisterRole request, CancellationToken cancellationToken)
    {
        var exists = await _context.Set<Role>().AnyAsync(r => r.NormalizedName == request.Name.ToUpper(), cancellationToken);

        if (exists)
        {
            return Response.Failure<string>("Role.Register.Duplicated", $"Role '{request.Name}' already exists");
        }

        var role = new Role
        {
            Name = request.Name,
            AssignByDefault = request.AssignByDefault,
            NormalizedName = request.Name.ToUpper()
        };

        _context.Add(role);
        await _context.SaveChangesAsync(cancellationToken);
        return Response.Success(role.Id);
    }
}