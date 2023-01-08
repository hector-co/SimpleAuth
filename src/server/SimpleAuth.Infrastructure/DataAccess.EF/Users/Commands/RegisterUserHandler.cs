using Mapster;
using Microsoft.EntityFrameworkCore;
using SimpleAuth.Domain.Model;
using SimpleAuth.Domain.Abstractions;
using SimpleAuth.Application.Abstractions.Commands;
using SimpleAuth.Application.Commands.Users;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Users.Commands;

public class RegisterUserHandler : ICommandHandler<RegisterUser, int>
{
    private readonly SimpleAuthContext _context;

    public RegisterUserHandler(SimpleAuthContext context)
    {
        _context = context;
    }

    public async Task<Response<int>> Handle(RegisterUser request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            IsAdmin = request.IsAdmin,
            UserName = request.UserName,
            Email = request.Email,
            EmailConfirmed = request.EmailConfirmed,
            PhoneNumber = request.PhoneNumber,
            DisplayName = request.DisplayName,
            Name = request.Name,
            LastName = request.LastName,
            Disabled = request.Disabled,
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