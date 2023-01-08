using Mapster;
using Microsoft.EntityFrameworkCore;
using SimpleAuth.Domain.Model;
using SimpleAuth.Domain.Abstractions;
using SimpleAuth.Application.Abstractions.Commands;
using SimpleAuth.Application.Commands.Users;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Users.Commands;

public class UpdateUserHandler : ICommandHandler<UpdateUser>
{
    private readonly SimpleAuthContext _context;

    public UpdateUserHandler(SimpleAuthContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(UpdateUser request, CancellationToken cancellationToken)
    {
        var user = await _context.Set<User>()
            .AddIncludes()
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (user == null)
            return Response.Failure(new Error("User.Update.NotFound", "Entity not found."));

        user.IsAdmin = request.IsAdmin;
        user.UserName = request.UserName;
        user.Email = request.Email;
        user.EmailConfirmed = request.EmailConfirmed;
        user.PhoneNumber = request.PhoneNumber;
        user.DisplayName = request.DisplayName;
        user.Name = request.Name;
        user.LastName = request.LastName;
        user.Disabled = request.Disabled;
        user.Roles = await _context.Set<Role>().Where(er => request.RolesId.Contains(er.Id)).ToListAsync(cancellationToken);
        user.Claims = request.Claims.Select(r => new UserClaim
        {
            Id = r.Id,
            ClaimType = r.ClaimType,
            ClaimValue = r.ClaimValue,
        }).ToList();

        await _context.SaveChangesAsync(cancellationToken);
        return Response.Success();
    }
}