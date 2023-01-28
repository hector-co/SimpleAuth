using SimpleAuth.Application.Common.Commands;

namespace SimpleAuth.Application.Users.Commands;

public record DeleteUser(Guid Id) : ICommand;
