using MediatR;
using SleshWrites.Domain.Common;

namespace SleshWrites.Application.Common.Interfaces;

/// <summary>
/// Handler for queries that return a Result with a value.
/// </summary>
/// <typeparam name="TQuery">The type of query to handle.</typeparam>
/// <typeparam name="TResponse">The type of value returned on success.</typeparam>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;
