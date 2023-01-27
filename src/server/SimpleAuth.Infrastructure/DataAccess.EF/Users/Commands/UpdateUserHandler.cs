using Microsoft.EntityFrameworkCore;
using SimpleAuth.Domain.Model;
using SimpleAuth.Domain.Common;
using SimpleAuth.Application.Common.Commands;
using SimpleAuth.Application.Users.Commands;
using Microsoft.AspNetCore.Identity;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Users.Commands;

public class UpdateUserHandler : ICommandHandler<UpdateUser>
{
    private readonly SimpleAuthContext _context;
    private readonly UserManager<User> _userManager;

    public UpdateUserHandler(SimpleAuthContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<Response> Handle(UpdateUser request, CancellationToken cancellationToken)
    {
        var user = await _context.Set<User>()
            .AddIncludes()
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (user == null)
            return Response.Failure(new Error("User.Update.NotFound", "Entity not found."));

        user.Name = request.Name;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;
        if (request.RolesId == null)
        {
            user.UserRoles = new List<UserRole>();
        }
        else
        {
            user.UserRoles = request.RolesId.Select(rId => new UserRole { RoleId = rId }).ToList();
        }
        user.Claims = new List<UserClaim>
            {
                new UserClaim { ClaimType = Claims.GivenName, ClaimValue = request.Name },
                new UserClaim { ClaimType = Claims.FamilyName, ClaimValue = request.LastName },
                new UserClaim { ClaimType = Claims.PhoneNumber, ClaimValue = request.PhoneNumber },
            };

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            if (result.Errors.Any(e => e.Code.Equals("DuplicateUserName", StringComparison.InvariantCultureIgnoreCase)))
            {
                return Response.Failure<string>("User.Update.Duplicated", $"USer '{request.Name}' already exists");
            }

            return Response.Failure<string>("User.Update.Error", result.ToString());
        }

        return Response.Success();
    }
}