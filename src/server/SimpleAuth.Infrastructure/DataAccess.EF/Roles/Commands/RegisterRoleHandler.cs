using Mapster;
using Microsoft.EntityFrameworkCore;
using SimpleAuth.Domain.Model;
using SimpleAuth.Domain.Abstractions;
using SimpleAuth.Application.Abstractions.Commands;
using SimpleAuth.Application.Commands.Roles;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Roles.Commands;

public class RegisterRoleHandler : ICommandHandler<RegisterRole, int>
{
    private readonly SimpleAuthContext _context;

    public RegisterRoleHandler(SimpleAuthContext context)
    {
        _context = context;
    }

    public async Task<Response<int>> Handle(RegisterRole request, CancellationToken cancellationToken)
    {
        var role = new Role
        {
            Name = request.Name,
            DefaultRole = request.DefaultRole,
            Disabled = request.Disabled,
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