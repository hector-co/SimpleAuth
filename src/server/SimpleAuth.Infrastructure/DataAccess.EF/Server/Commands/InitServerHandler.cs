using Microsoft.AspNetCore.Identity;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using SimpleAuth.Application.Common.Commands;
using SimpleAuth.Application.Server.Commands;
using SimpleAuth.Domain.Common;
using SimpleAuth.Domain.Model;
using static OpenIddict.Abstractions.OpenIddictConstants;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SimpleAuth.Application.Server;
using Microsoft.Extensions.Options;
using SimpleAuth.Application;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Server.Commands;

public class InitServerHandler : ICommandHandler<InitServer>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IUserStore<User> _userStore;
    private readonly OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication<int>> _appManager;
    private readonly OpenIddictScopeManager<OpenIddictEntityFrameworkCoreScope<int>> _scopeManager;
    private readonly SimpleAuthContext _context;
    private readonly ServerSettingsOption _serverOptions;

    public InitServerHandler(UserManager<User> userManager, RoleManager<Role> roleManager,
        IUserStore<User> userStore, OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication<int>> appManager,
        OpenIddictScopeManager<OpenIddictEntityFrameworkCoreScope<int>> scopeManager, SimpleAuthContext context,
        IOptions<ServerSettingsOption> serverOptions)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _userStore = userStore;
        _appManager = appManager;
        _scopeManager = scopeManager;
        _context = context;
        _serverOptions = serverOptions.Value;
    }

    public async Task<Response> Handle(InitServer request, CancellationToken cancellationToken)
    {
        await RegisterDefaults(cancellationToken);

        await ProcessServerSetup(request.ServerSetup, cancellationToken);

        return Response.Success();
    }

    private async Task RegisterDefaults(CancellationToken cancellationToken)
    {
        if (!await _context.Set<ServerSettings>().AnyAsync(cancellationToken))
        {
            _context.Add(new ServerSettings(true));
            await _context.SaveChangesAsync(cancellationToken);
        }

        await SetupRoles(new List<ServerSetup.SetupRole> { new ServerSetup.SetupRole(AuthConstants.Roles.AuthAdmin, false) });

        var authRole = await _context.Set<Role>()
            .FirstAsync(r => r.NormalizedName == AuthConstants.Roles.AuthAdmin.ToUpper(), cancellationToken);

        if (!await _context.Set<User>().AnyAsync(u => u.UserRoles.Any(r => r.Role.Id == authRole.Id), cancellationToken))
        {
            var adminUser = new User
            {
                EmailConfirmed = true,
                Email = _serverOptions.AdminEmail,
                PhoneNumber = string.Empty,
                Claims = new List<UserClaim>
                    {
                        new UserClaim { ClaimType = Claims.GivenName, ClaimValue = string.Empty },
                        new UserClaim { ClaimType = Claims.FamilyName, ClaimValue = string.Empty },
                        new UserClaim { ClaimType = Claims.PhoneNumber, ClaimValue = string.Empty }
                    },
                UserRoles = new List<UserRole> { new UserRole { Role = authRole } }
            };

            await _userStore.SetUserNameAsync(adminUser, _serverOptions.AdminEmail, cancellationToken);
            await ((IUserEmailStore<User>)_userStore).SetEmailAsync(adminUser, _serverOptions.AdminEmail, cancellationToken);

            await _userManager.CreateAsync(adminUser, _serverOptions.AdminPassword);
        }

        await SetupPublicApps(
            new List<ServerSetup.PublicApp> {
                new ServerSetup.PublicApp(
                    "swagger-ui",
                    "Swagger UI",
                    new[]{ $"{_serverOptions.ServerUrl}/swagger/oauth2-redirect.html" }.ToList(),
                    new []{ "" }.ToList(), new []{ "openid", "profile" }.ToList())},
            cancellationToken);
    }

    private async Task ProcessServerSetup(ServerSetup? serverSetup, CancellationToken cancellationToken)
    {
        if (serverSetup == null) return;

        var setting = await _context.Set<ServerSettings>().FirstAsync(cancellationToken);
        setting.AllowSelfRegistration = serverSetup.AllowSelfRegistration;
        await _context.SaveChangesAsync(cancellationToken);
        await SetupRoles(serverSetup.Roles);

        var scopes = serverSetup.Scopes.ToDictionary(s => s.Name, s => (s.DisplayName, Resources: new List<string>()));
        var appScopes = serverSetup.ConfidentialApps
            .Select(a => (a.Id, a.Scopes, Confidential: true))
            .Concat(serverSetup.PublicApps.Select(a => (a.Id, a.Scopes, Confidential: false)));
        foreach (var appScope in appScopes)
        {
            foreach (var scope in appScope.Scopes)
            {
                if (!scopes.ContainsKey(scope))
                {
                    scopes.Add(scope, (scope, new List<string>()));
                }
                if (appScope.Confidential)
                {
                    scopes[scope].Resources.Add(appScope.Id);
                }
            }
        }

        await SetupScopes(scopes, cancellationToken);
        await SetupConfidentialApps(serverSetup.ConfidentialApps, cancellationToken);
        await SetupPublicApps(serverSetup.PublicApps, cancellationToken);
    }

    private async Task SetupRoles(List<ServerSetup.SetupRole> roles)
    {
        foreach (var roleInfo in roles)
        {
            var existentRole = await _roleManager.FindByNameAsync(roleInfo.Name);
            if (existentRole == null)
            {
                await _roleManager.CreateAsync(new Role
                {
                    Name = roleInfo.Name,
                    AssignByDefault = roleInfo.AssignByDefault
                });
            }
            else
            {
                existentRole.AssignByDefault = roleInfo.AssignByDefault;
                await _roleManager.UpdateAsync(existentRole);
            }
        }
    }

    private async Task SetupScopes(Dictionary<string, (string displayName, List<string> resources)> scopes,
        CancellationToken cancellationToken)
    {
        foreach (var tuple in scopes)
        {
            var scope = await _scopeManager.FindByNameAsync(tuple.Key, cancellationToken);

            if (scope == null)
            {
                scope = new OpenIddictEntityFrameworkCoreScope<int>
                {
                    Name = tuple.Key,
                    DisplayName = tuple.Value.displayName,
                    Resources = JsonSerializer.Serialize(tuple.Value.resources)
                };

                await _scopeManager.CreateAsync(scope, cancellationToken);
            }
            else
            {
                scope.Resources = JsonSerializer.Serialize(tuple.Value.resources);
                scope.DisplayName = tuple.Value.displayName;
                await _scopeManager.UpdateAsync(scope, cancellationToken);
            }
        }
    }

    private async Task SetupConfidentialApps(List<ServerSetup.ConfidentialApp> apps, CancellationToken cancellationToken)
    {
        foreach (var appInfo in apps)
        {
            var app = await _appManager.FindByClientIdAsync(appInfo.Id, cancellationToken);

            var isNew = false;

            if (app == null)
            {
                app = new OpenIddictEntityFrameworkCoreApplication<int>
                {
                    ClientId = appInfo.Id,
                    ConsentType = ConsentTypes.Implicit,
                    Type = ClientTypes.Confidential,
                    Permissions = JsonSerializer.Serialize(new[] { Permissions.Endpoints.Introspection })
                };
                isNew = true;
            }

            app.DisplayName = appInfo.DisplayName;

            if (isNew)
            {
                await _appManager.CreateAsync(app, appInfo.Secret, cancellationToken);
            }
            else
            {
                await _appManager.UpdateAsync(app, appInfo.Secret, cancellationToken);
            }
        }
    }

    private async Task SetupPublicApps(List<ServerSetup.PublicApp> apps, CancellationToken cancellationToken)
    {
        foreach (var appInfo in apps)
        {
            var app = await _appManager.FindByClientIdAsync(appInfo.Id, cancellationToken);

            var defaultPermissions = new[]
            {
                Permissions.Endpoints.Token,
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Logout,

                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.RefreshToken,

                Permissions.ResponseTypes.IdToken,
                Permissions.ResponseTypes.IdTokenToken,
                Permissions.ResponseTypes.Token,
                Permissions.ResponseTypes.Code,

                Permissions.Scopes.Email,
                Permissions.Scopes.Profile,
                Permissions.Scopes.Roles
            };

            var permissions = new HashSet<string>(defaultPermissions.Concat(appInfo.Scopes.Select(s => Permissions.Prefixes.Scope + s)));

            var isNew = false;

            if (app == null)
            {
                app = new OpenIddictEntityFrameworkCoreApplication<int>
                {
                    ClientId = appInfo.Id,
                    ConsentType = ConsentTypes.Implicit,
                    Type = ClientTypes.Public
                };
                isNew = true;
            }

            app.DisplayName = appInfo.DisplayName;
            app.RedirectUris = JsonSerializer.Serialize(appInfo.RedirectUris);
            app.PostLogoutRedirectUris = JsonSerializer.Serialize(appInfo.PostLogoutRedirectUris);
            app.Permissions = JsonSerializer.Serialize(permissions);

            if (isNew)
            {
                await _appManager.CreateAsync(app, cancellationToken);
            }
            else
            {
                await _appManager.UpdateAsync(app, cancellationToken);
            }
        }
    }
}
