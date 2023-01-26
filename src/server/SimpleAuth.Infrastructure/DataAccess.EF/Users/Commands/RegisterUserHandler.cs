using SimpleAuth.Domain.Model;
using SimpleAuth.Domain.Common;
using SimpleAuth.Application.Common.Commands;
using SimpleAuth.Application.Users.Commands;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Users.Commands;

public class RegisterUserHandler : ICommandHandler<RegisterUser, string>
{
    private readonly SimpleAuthContext _context;
    private readonly UserManager<User> _userManager;

    public RegisterUserHandler(SimpleAuthContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<Response<string>> Handle(RegisterUser request, CancellationToken cancellationToken)
    {
        var defaultRoleIds = await _context.Set<Role>()
            .Where(r => r.AssignByDefault)
            .Select(r => r.Id).ToListAsync(cancellationToken);

        var roleIds = request.RolesId.Concat(defaultRoleIds).Distinct().ToList();

        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            Name = request.Name,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            UserRoles = roleIds.Select(rId => new UserRole { RoleId = rId }).ToList(),
            Claims = new List<UserClaim>
            {
                new UserClaim { ClaimType = Claims.GivenName, ClaimValue = request.Name },
                new UserClaim { ClaimType = Claims.FamilyName, ClaimValue = request.LastName },
                new UserClaim { ClaimType = Claims.PhoneNumber, ClaimValue = request.PhoneNumber },
            }
        };

        var result = await _userManager.CreateAsync(user);

        if (!result.Succeeded)
        {
            if (result.Errors.Any(e => e.Code.Equals("DuplicateUserName", StringComparison.InvariantCultureIgnoreCase)))
            {
                return Response.Failure<string>("User.Register.Duplicated", $"USer '{request.Name}' already exists");
            }

            return Response.Failure<string>("User.Register.Error", result.ToString());
        }

        return Response.Success(user.Id);
    }
}