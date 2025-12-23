using MediatR;
using SleshWrites.Domain.Common;

namespace SleshWrites.Application.Common.Interfaces;

/// <summary>
/// Marker interface for commands that return a Result.
/// </summary>
public interface ICommand : IRequest<Result>;

/// <summary>
/// Marker interface for commands that return a Result with a value.
/// </summary>
/// <typeparam name="TResponse">The type of value returned on success.</typeparam>
public interface ICommand<TResponse> : IRequest<Result<TResponse>>;
