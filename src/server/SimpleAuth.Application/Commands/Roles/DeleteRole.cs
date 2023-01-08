using SimpleAuth.Application.Abstractions.Commands;

namespace SimpleAuth.Application.Commands.Roles;

public record DeleteRole(int Id) : ICommand;
