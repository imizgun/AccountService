using Microsoft.AspNetCore.HttpLogging;

namespace AccountService.Extensions.BuilderExtensions;

public static class LoggingExtensions
{
    public static void AddCustomLogging(this IServiceCollection services)
    {
        services.AddHttpLogging(logging =>
        {
            logging.LoggingFields =
                HttpLoggingFields.RequestProtocol |
                HttpLoggingFields.RequestMethod |
                HttpLoggingFields.RequestPath |
                HttpLoggingFields.RequestQuery |
                HttpLoggingFields.RequestHeaders |
                HttpLoggingFields.RequestBody |
                HttpLoggingFields.ResponseStatusCode |
                HttpLoggingFields.ResponseHeaders |
                HttpLoggingFields.ResponseBody |
                HttpLoggingFields.Duration;

            logging.RequestBodyLogLimit = 4096;
            logging.ResponseBodyLogLimit = 4096;

            logging.MediaTypeOptions.AddText("application/json");
            logging.MediaTypeOptions.AddText("application/xml");
            logging.MediaTypeOptions.AddText("text/plain");
        });
    }
}