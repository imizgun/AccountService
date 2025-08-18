namespace AccountService.Extensions.AppExtensions;

public static class SwaggerAppExtensions
{
    public static void UseCustomSwagger(this WebApplication app)
    {
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}