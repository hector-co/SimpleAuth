using SimpleAuth.Domain.Model;
using SimpleAuth.Domain.Common;
using SimpleAuth.Application.Common.Commands;
using SimpleAuth.Application.Users.Commands;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Microsoft.Extensions.Localization;
using SimpleAuth.Application.Resources;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using SimpleAuth.Application;
using System.Web;
using SimpleAuth.Application.Server;
using Microsoft.Extensions.Options;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Users.Commands;

public class RegisterUserHandler : ICommandHandler<RegisterUser, Guid>
{
    private readonly SimpleAuthContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IStringLocalizer<EmailResource> _emailLocalizer;
    private readonly IEmailSender _emailSender;
    private readonly ServerSettingsOption _serverSettings;

    public RegisterUserHandler(SimpleAuthContext context, UserManager<User> userManager,
        IStringLocalizer<EmailResource> stringLocalizer, IEmailSender emailSender, IOptions<ServerSettingsOption> serverSettings)
    {
        _context = context;
        _userManager = userManager;
        _emailLocalizer = stringLocalizer;
        _emailSender = emailSender;
        _serverSettings = serverSettings.Value;
    }

    public async Task<Response<Guid>> Handle(RegisterUser request, CancellationToken cancellationToken)
    {
        var roleIds = request.RolesId ?? new List<Guid>();

        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = true,
            Name = request.Name,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber ?? string.Empty,
            UserRoles = roleIds.Select(rId => new UserRole { RoleId = rId }).ToList(),
            LockoutEnabled = true,
            Claims = new List<UserClaim>
            {
                new UserClaim { ClaimType = Claims.GivenName, ClaimValue = request.Name },
                new UserClaim { ClaimType = Claims.FamilyName, ClaimValue = request.LastName },
                new UserClaim { ClaimType = Claims.PhoneNumber, ClaimValue = request.PhoneNumber ?? string.Empty },
            }
        };

        var result = await _userManager.CreateAsync(user, Guid.NewGuid().ToString());

        if (!result.Succeeded)
        {
            if (result.Errors.Any(e => e.Code.Equals("DuplicateUserName", StringComparison.InvariantCultureIgnoreCase)))
            {
                return Response.Failure<Guid>("User.Register.Duplicated", "Duplicated user email");
            }

            return Response.Failure<Guid>("User.Register.Error", result.ToString());
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var link = $"{_serverSettings.ServerUrl}/Account/ResetPassword?code={HttpUtility.UrlEncode(token)}";
        await _emailSender.SendEmailAsync(
            user.Email,
            _emailLocalizer["Confirm your email"],
            _emailLocalizer["ConfirmEmailMessage", link]);

        return Response.Success(user.Id);
    }
}