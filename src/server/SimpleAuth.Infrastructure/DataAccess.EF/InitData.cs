using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SimpleAuth.Application.Server;
using SimpleAuth.Application.Server.Commands;
using SimpleAuth.Domain.Model;
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
        var context = serviceProvider.GetRequiredService<SimpleAuthContext>();
        if (!await context.Set<ServerSettings>().AnyAsync(cancellationToken))
        {
            context.Add(new ServerSettings(true));
            await context.SaveChangesAsync(cancellationToken);
        }

        var serverSettings = serviceProvider.GetRequiredService<IOptions<ServerSettingsOption>>().Value;
        var settingsFileName = $"{serverSettings.SetupFilePath}/auth-server-settings.json";
        if (!File.Exists(settingsFileName))
            return;

        var mediator = serviceProvider.GetRequiredService<IMediator>();

        var fileContent = File.ReadAllText(settingsFileName);
        var command = JsonSerializer.Deserialize<SetupServer>(fileContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (command == null)
            return;

        await mediator.Send(command, cancellationToken);
    }
}

