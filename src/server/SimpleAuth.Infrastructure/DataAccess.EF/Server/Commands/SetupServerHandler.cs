using Microsoft.AspNetCore.Identity;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using SimpleAuth.Domain.Common;
using SimpleAuth.Domain.Model;
using static OpenIddict.Abstractions.OpenIddictConstants;
using System.Text.Json;
using SimpleAuth.Application.Common.Commands;
using SimpleAuth.Application.Server.Commands;
using Microsoft.EntityFrameworkCore;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Server.Commands;

public class SetupServerHandler : ICommandHandler<SetupServer>
{
    private readonly RoleManager<Role> _roleManager;
    private readonly OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication<int>> _appManager;
    private readonly OpenIddictScopeManager<OpenIddictEntityFrameworkCoreScope<int>> _scopeManager;
    private readonly SimpleAuthContext _context;

    public SetupServerHandler(RoleManager<Role> roleManager, OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication<int>> appManager,
        OpenIddictScopeManager<OpenIddictEntityFrameworkCoreScope<int>> scopeManager, SimpleAuthContext context)
    {
        _roleManager = roleManager;
        _appManager = appManager;
        _scopeManager = scopeManager;
        _context = context;
    }

    public async Task<Response> Handle(SetupServer request, CancellationToken cancellationToken)
    {
        var setting = await _context.Set<ServerSettings>().FirstAsync(cancellationToken);
        setting.AllowSelfRegistration = request.AllowSelfRegistration;
        await _context.SaveChangesAsync(cancellationToken);

        await SetupRoles(request.Roles);

        var scopes = request.Scopes.ToDictionary(s => s.Name, s => (s.DisplayName, Resources: new List<string>()));
        var appScopes = request.ConfidentialApps
            .Select(a => (a.Id, a.Scopes, Confidential: true))
            .Concat(request.PublicApps.Select(a => (a.Id, a.Scopes, Confidential: false)));
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
        await SetupConfidentialApps(request.ConfidentialApps, cancellationToken);
        await SetupPublicApps(request.PublicApps, cancellationToken);

        return Response.Success();
    }

    private async Task SetupRoles(List<SetupServer.SetupRole> roles)
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

    private async Task SetupConfidentialApps(List<SetupServer.ConfidentialApp> apps, CancellationToken cancellationToken)
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

    private async Task SetupPublicApps(List<SetupServer.PublicApp> apps, CancellationToken cancellationToken)
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
