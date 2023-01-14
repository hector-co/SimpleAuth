using SimpleAuth.Application.Common.Commands;

namespace SimpleAuth.Application.Users.Commands;

public record DeleteUser(string Id) : ICommand;
