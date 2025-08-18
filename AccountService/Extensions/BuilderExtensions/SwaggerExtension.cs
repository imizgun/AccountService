using Microsoft.OpenApi.Models;

namespace AccountService.Extensions.BuilderExtensions;

public static class SwaggerExtensions
{
    public static void AddCustomSwagger(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            var mainXml = Path.Combine(AppContext.BaseDirectory, "AccountService.xml");
            var appXml = Path.Combine(AppContext.BaseDirectory, "AccountService.Application.xml");

            c.IncludeXmlComments(mainXml);
            c.IncludeXmlComments(appXml);

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Введите токен в формате: Bearer {your JWT token}",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });
            
            c.RegisterEventSchemas();
            c.DocumentFilter<SwaggerEventsExtensions.EventsDocumentFilter>();

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }
}