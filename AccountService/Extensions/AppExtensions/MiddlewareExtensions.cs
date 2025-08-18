using Serilog;

namespace AccountService.Extensions.AppExtensions;

public static class MiddlewareExtensions
{
    public static void UseCustomMiddlewares(this WebApplication app)
    {
        app.UseHttpLogging();

        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate =
                "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value!);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.FirstOrDefault()!);
                diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString()!);

                if (httpContext.User.Identity?.IsAuthenticated == true)
                {
                    diagnosticContext.Set("UserId", httpContext.User.Identity.Name!);
                }
            };
        });

        app.UseExceptionHandler();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.UseCors(c => c
            .AllowAnyHeader()
            .AllowAnyOrigin()
            .AllowAnyMethod()
        );

        app.UseHttpsRedirection();
    }
}