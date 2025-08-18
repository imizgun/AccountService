using AccountService.Application.Features.Accounts.Events;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AccountService.Extensions.BuilderExtensions;

public static class SwaggerEventsExtensions
{
	public static void RegisterEventSchemas(this SwaggerGenOptions c)
	{
		var assemblies = new[]
		{
			typeof(AccountClosed).Assembly
		};

		foreach (var asm in assemblies)
		{
			var eventTypes = asm.GetTypes()
				.Where(t => t is {IsClass: true, IsAbstract: false} && t.Namespace?.Contains("Events") == true);

			foreach (var type in eventTypes)
			{
				c.SchemaGeneratorOptions.SchemaFilters.Add(new DummySchemaFilter(type));
			}
		}
	}

	private class DummySchemaFilter(Type type) : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
		{
			if (context.Type == type)
			{
			}
		}
	}
	
	public class EventsDocumentFilter : IDocumentFilter
	{
		public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
		{
			var eventTypes = typeof(AccountClosed).Assembly
				.GetTypes()
				.Where(t => t is { IsClass: true, IsAbstract: false} && t.Namespace?.Contains("Events") == true);

			foreach (var type in eventTypes)
			{
				context.SchemaGenerator.GenerateSchema(type, context.SchemaRepository);
			}
		}
	}

}