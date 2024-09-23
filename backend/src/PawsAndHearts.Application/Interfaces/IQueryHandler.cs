﻿using CSharpFunctionalExtensions;
using PawsAndHearts.Domain.Shared;

namespace PawsAndHearts.Application.Interfaces;

public interface IQueryHandler<TResponse, in TQuery> where TQuery : IQuery
{
    public Task<TResponse> Handle(TQuery command, CancellationToken cancellationToken = default);
}

public interface IQueryHandlerWithResult<TResponse, in TQuery> where TQuery : IQuery
{
    public Task<Result<TResponse, ErrorList>> Handle(TQuery command, CancellationToken cancellationToken = default);
}