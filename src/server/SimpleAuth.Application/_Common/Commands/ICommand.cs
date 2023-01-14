using MediatR;
using SimpleAuth.Domain.Common;

namespace SimpleAuth.Application.Common.Commands;

public interface ICommand : IRequest<Response>
{
}

public interface ICommand<TValue> : IRequest<Response<TValue>>

{
}
