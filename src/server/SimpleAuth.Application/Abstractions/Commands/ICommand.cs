using MediatR;
using SimpleAuth.Domain.Abstractions;

namespace SimpleAuth.Application.Abstractions.Commands;

public interface ICommand : IRequest<Response>
{
}

public interface ICommand<TValue> : IRequest<Response<TValue>>

{
}
