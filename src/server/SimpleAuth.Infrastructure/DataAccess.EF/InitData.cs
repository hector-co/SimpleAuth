using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SimpleAuth.Application.Server;
using SimpleAuth.Application.Server.Commands;
using System.Text.Json;

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
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        var serverSettings = serviceProvider.GetRequiredService<IOptions<ServerSettingsOption>>().Value;
        var settingsFileName = $"{serverSettings.SetupFilePath}/auth-server-settings.json";

        var fileContent = File.Exists(settingsFileName)
            ? File.ReadAllText(settingsFileName)
            : null;

        var setup = string.IsNullOrEmpty(fileContent)
            ? null
            : JsonSerializer.Deserialize<ServerSetup>(fileContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        var command = new InitServer(setup);

        await mediator.Send(command, cancellationToken);
    }
}

