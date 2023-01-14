using MediatR;

namespace SimpleAuth.Application.Common.Queries;

public interface IQuery<TData> : IRequest<Result<TData>>
{
}

