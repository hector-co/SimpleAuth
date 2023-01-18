using Mapster;
using Microsoft.EntityFrameworkCore;
using SimpleAuth.Domain.Model;
using SimpleAuth.Domain.Common;
using SimpleAuth.Application.Common.Commands;
using SimpleAuth.Application.Users.Commands;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Users.Commands;

public class RegisterUserHandler : ICommandHandler<RegisterUser, string>
{
    private readonly SimpleAuthContext _context;

    public RegisterUserHandler(SimpleAuthContext context)
    {
        _context = context;
    }

    public async Task<Response<string>> Handle(RegisterUser request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            EmailConfirmed = request.EmailConfirmed,
            PhoneNumber = request.PhoneNumber,
            Name = request.Name,
            LastName = request.LastName,
            Roles = await _context.Set<Role>().Where(er => request.RolesId.Contains(er.Id)).ToListAsync(cancellationToken),
            Claims = request.Claims.Select(r => new UserClaim
            {
                ClaimType = r.ClaimType,
                ClaimValue = r.ClaimValue,
            }).ToList(),
        };

        _context.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
        return Response.Success(user.Id);
    }
}