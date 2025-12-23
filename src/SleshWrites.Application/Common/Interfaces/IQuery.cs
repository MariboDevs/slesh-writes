using MediatR;
using SleshWrites.Domain.Common;

namespace SleshWrites.Application.Common.Interfaces;

/// <summary>
/// Marker interface for queries that return a Result with a value.
/// </summary>
/// <typeparam name="TResponse">The type of value returned on success.</typeparam>
public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
