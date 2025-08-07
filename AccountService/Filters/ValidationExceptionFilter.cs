using AccountService.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AccountService.Filters;

public class ValidationExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not ValidationException validationException) return;

        var problemDetails = MbResult<string>.ValidationFail(
            validationException.Errors
                .Select(x => x.ToString()).ToList());

        context.Result = new BadRequestObjectResult(problemDetails);
        context.ExceptionHandled = true;
    }
}