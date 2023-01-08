using MediatR;

namespace SimpleAuth.Application.Abstractions.Queries;

public interface IQuery<TData> : IRequest<Result<TData>>
{
}

