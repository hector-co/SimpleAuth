using SimpleAuth.Application.Common.Commands;

namespace SimpleAuth.Application.Server.Commands;

public record InitServer(ServerSetup? ServerSetup) : ICommand;
