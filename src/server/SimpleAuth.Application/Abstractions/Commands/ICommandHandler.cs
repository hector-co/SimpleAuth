using MediatR;
using SimpleAuth.Domain.Abstractions;

namespace SimpleAuth.Application.Abstractions.Commands;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Response>
    where TCommand : ICommand
{
}

public interface ICommandHandler<in TCommand, TValue> : IRequestHandler<TCommand, Response<TValue>>
    where TCommand : ICommand<TValue>
{
}