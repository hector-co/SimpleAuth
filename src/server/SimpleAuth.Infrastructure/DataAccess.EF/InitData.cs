using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
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
        //var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();

        await context.Database.MigrateAsync(cancellationToken);

        await AddData(scope.ServiceProvider, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static async Task AddData(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

        if (!await roleManager.RoleExistsAsync("Customer"))
        {
            await roleManager.CreateAsync(new Role
            {
                Name = "Customer",
                AssignByDefault = true
            });
        }

        var manager = serviceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        if (await manager.FindByClientIdAsync("pictures-webapp", cancellationToken) == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "pictures-webapp",
                ConsentType = ConsentTypes.Implicit,
                Type = ClientTypes.Public,
                DisplayName = "MVC client application",
                RedirectUris =
                {
                    new Uri("http://localhost:8080"),
                    new Uri("http://localhost:8080/signin-callback"),
                    new Uri("http://localhost:8080/silent-callback.html")
                },
                PostLogoutRedirectUris =
                {
                    new Uri("http://localhost:8080")
                },
                Permissions =
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
                    Permissions.Scopes.Roles,

                    Permissions.Prefixes.Scope + "pictures-api"
                }
            }, cancellationToken);
        }

        if (await manager.FindByClientIdAsync("pictures-api", cancellationToken) == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "pictures-api",
                ClientSecret = "20DC15F7-BB3D-4741-BEBC-83D797363CAF",
                ConsentType = ConsentTypes.Implicit,
                Type = ClientTypes.Confidential,
                DisplayName = "Pictures api application",
                Permissions =
                {
                    Permissions.Endpoints.Introspection
                }
            }, cancellationToken);
        }

        var scopeManager = serviceProvider.GetRequiredService<OpenIddictScopeManager<OpenIddictEntityFrameworkCoreScope<int>>>();
        if (await scopeManager.FindByNameAsync("pictures-api", cancellationToken) == null)
        {
            var scopeDescriptor = new OpenIddictEntityFrameworkCoreScope<int>
            {
                Name = "pictures-api",
                Resources = JsonSerializer.Serialize(new[] { "pictures-api" })
            };

            await scopeManager.CreateAsync(scopeDescriptor, cancellationToken);
        }
    }
}

