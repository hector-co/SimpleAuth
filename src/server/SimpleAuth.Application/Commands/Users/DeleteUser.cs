using SimpleAuth.Application.Abstractions.Commands;

namespace SimpleAuth.Application.Commands.Users;

public record DeleteUser(int Id) : ICommand;
