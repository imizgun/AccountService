using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using AccountService.Responses;

namespace AccountService.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context, 
        Exception exception, 
        CancellationToken cancellationToken)
    {

        var (statusCode, response) = MapExceptionToResponse(exception);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }

    private static (int StatusCode, MbResult<object> Response) MapExceptionToResponse(Exception exception)
    {
        return exception switch
        {
            DbUpdateConcurrencyException => (
                StatusCode: StatusCodes.Status409Conflict,
                Response: MbResult<object>.Fail("Concurrency conflict")
            ),

            DbUpdateException ex when Utils.Utils.TryGetPg(ex, out var pg) && (pg.SqlState == "40001" || pg.SqlState == "40P01") => (
                StatusCode: StatusCodes.Status409Conflict,
                Response: MbResult<object>.Fail($"DB conflict ({pg.SqlState})")
            ),

            InvalidOperationException ex when ex.Message.Contains("transient failure", StringComparison.OrdinalIgnoreCase) => (
                StatusCode: StatusCodes.Status409Conflict,
                Response: MbResult<object>.Fail("Transient failure, please retry")
            ),
            
            KeyNotFoundException => (
                StatusCode: StatusCodes.Status404NotFound,
                Response: MbResult<object>.Fail(exception.Message)
            ),

            _ => (
                StatusCode: StatusCodes.Status400BadRequest,
                Response: MbResult<object>.Fail(exception.Message)
            )
        };
    }
}