using SimpleAuth.Application.Abstractions.Commands;

namespace SimpleAuth.Application.Commands.Roles;

public record DeleteRole(string Id) : ICommand;
