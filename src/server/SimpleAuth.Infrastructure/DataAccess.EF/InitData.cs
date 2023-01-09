using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
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
                }
            }, cancellationToken);
        }
    }
}

