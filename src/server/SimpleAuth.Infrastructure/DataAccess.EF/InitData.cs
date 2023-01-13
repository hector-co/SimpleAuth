using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using SimpleAuth.Domain.Model;
using System.Text.Json;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace SimpleAuth.Infrastructure.DataAccess.EF;

public class InitData : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public InitData(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SimpleAuthContext>();

        await context.Database.MigrateAsync(cancellationToken);

        await AddData(scope.ServiceProvider, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static async Task AddData(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        //await SetupServer(serviceProvider, cancellationToken);

        await SetupTestApplication(serviceProvider, cancellationToken);
    }

    private static async Task SetupServer(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
        const string AuthAdminRole = "AuthAdmin";

        if (!await roleManager.RoleExistsAsync(AuthAdminRole))
        {
            await roleManager.CreateAsync(new Role
            {
                Name = AuthAdminRole
            });
        }

        await CreateOrUpdateScope(serviceProvider, "auth-api", "Auth API", new[] { "Auth Resources" }, cancellationToken);
        await CreateOrUpdateConfidentialClient(serviceProvider, "auth-resources", "", "Auth Resources", cancellationToken);
        await CreateOrUpdatePublicClient(serviceProvider, "auth-management-webapp", "Auth Management Webapp",
            new[]
            {
                new Uri("http://localhost:8080"),
                new Uri("http://localhost:8080/signin-callback"),
                new Uri("http://localhost:8080/silent-callback.html")
            },
            new[]
            {
                new Uri("http://localhost:8080"),
                new Uri("http://localhost:8080/signin-callback")
            }, new[] { "auth-api" }, cancellationToken);
    }

    private static async Task SetupTestApplication(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
        const string CustomerRole = "Customer";

        if (!await roleManager.RoleExistsAsync(CustomerRole))
        {
            await roleManager.CreateAsync(new Role
            {
                Name = CustomerRole,
                AssignByDefault = true
            });
        }

        await CreateOrUpdateScope(serviceProvider, "pictures-api", "Pictures API", new[] { "pictures-resources" }, cancellationToken);
        await CreateOrUpdateConfidentialClient(serviceProvider, "pictures-resources", "20DC15F7-BB3D-4741-BEBC-83D797363CAF", "Pictures Resources", cancellationToken);
        await CreateOrUpdatePublicClient(serviceProvider, "pictures-webapp", "Pictures Webapp",
            new[]
            {
                new Uri("http://localhost:8080"),
                new Uri("http://localhost:8080/signin-callback"),
                new Uri("http://localhost:8080/silent-callback.html")
            },
            new[] { new Uri("http://localhost:8080") },
            new[] { Permissions.Prefixes.Scope + "pictures-api" }, cancellationToken);
    }

    private static async Task CreateOrUpdateConfidentialClient(IServiceProvider serviceProvider, string clientId, string clientSecret, string displayName,
        CancellationToken cancellationToken = default)
    {
        var manager = serviceProvider.GetRequiredService<OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication<int>>>();

        var app = await manager.FindByClientIdAsync(clientId, cancellationToken);

        var isNew = false;

        if (app == null)
        {
            app = new OpenIddictEntityFrameworkCoreApplication<int>
            {
                ClientId = clientId,
                ConsentType = ConsentTypes.Implicit,
                Type = ClientTypes.Confidential,
                Permissions = JsonSerializer.Serialize(new[] { Permissions.Endpoints.Introspection })
            };
            isNew = true;
        }

        app.DisplayName = displayName;

        if (isNew)
        {
            await manager.CreateAsync(app, clientSecret, cancellationToken);
        }
        else
        {
            await manager.UpdateAsync(app, clientSecret, cancellationToken);
        }
    }

    private static async Task CreateOrUpdatePublicClient(IServiceProvider serviceProvider, string clientId, string displayName,
        IEnumerable<Uri> redirectUris, IEnumerable<Uri> postLogoutRedirectUris, IEnumerable<string> scopes, CancellationToken cancellationToken = default)
    {
        var manager = serviceProvider.GetRequiredService<OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication<int>>>();

        var app = await manager.FindByClientIdAsync(clientId, cancellationToken);

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

        var permissions = new HashSet<string>(defaultPermissions.Concat(scopes));

        var isNew = false;

        if (app == null)
        {
            app = new OpenIddictEntityFrameworkCoreApplication<int>
            {
                ClientId = clientId,
                ConsentType = ConsentTypes.Implicit,
                Type = ClientTypes.Public
            };
            isNew = true;
        }

        app.DisplayName = displayName;
        app.RedirectUris = JsonSerializer.Serialize(redirectUris);
        app.PostLogoutRedirectUris = JsonSerializer.Serialize(postLogoutRedirectUris);
        app.Permissions = JsonSerializer.Serialize(permissions);

        if (isNew)
        {
            await manager.CreateAsync(app, cancellationToken);
        }
        else
        {
            await manager.UpdateAsync(app, cancellationToken);
        }
    }

    private static async Task CreateOrUpdateScope(IServiceProvider serviceProvider, string name, string displayName, IEnumerable<string> resources, CancellationToken cancellationToken = default)
    {
        var scopeManager = serviceProvider.GetRequiredService<OpenIddictScopeManager<OpenIddictEntityFrameworkCoreScope<int>>>();

        var scope = await scopeManager.FindByNameAsync(name, cancellationToken);

        if (scope == null)
        {
            scope = new OpenIddictEntityFrameworkCoreScope<int>
            {
                Name = name,
                DisplayName = displayName,
                Resources = JsonSerializer.Serialize(resources)
            };

            await scopeManager.CreateAsync(scope, cancellationToken);
        }
        else
        {
            scope.Resources = JsonSerializer.Serialize(resources);
            scope.DisplayName = displayName;
            await scopeManager.UpdateAsync(scope, cancellationToken);
        }
    }
}

