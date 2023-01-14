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
        var role = new Role
        {
            Name = request.Name,
            AssignByDefault = request.AssignByDefault,
            Claims = request.Claims.Select(r => new RoleClaim
            {
                ClaimType = r.ClaimType,
                ClaimValue = r.ClaimValue,
            }).ToList(),
        };

        _context.Add(role);
        await _context.SaveChangesAsync(cancellationToken);
        return Response.Success(role.Id);
    }
}