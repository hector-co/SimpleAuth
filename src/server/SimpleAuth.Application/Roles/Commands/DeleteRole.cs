using SimpleAuth.Application.Common.Commands;

namespace SimpleAuth.Application.Roles.Commands;

public record DeleteRole(Guid Id) : ICommand;
