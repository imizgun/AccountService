﻿using FluentValidation;
using MediatR;

namespace AccountService.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task
            .WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var error = validationResults
            .SelectMany(r => r.Errors)
            .FirstOrDefault(r => r != null);

        if (error is not null)
            throw new ValidationException([error]);


        return await next(cancellationToken);
    }
}