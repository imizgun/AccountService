using AccountService.Application.Shared;
using AccountService.Background.Rabbit;
using AccountService.Extensions.AppExtensions;
using AccountService.Extensions.BuilderExtensions;
using AccountService.Filters;
using AccountService.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, services, cfg) =>
    cfg.ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

builder.Services.AddCustomLogging();

builder.Services.AddCustomSwagger();

builder.Services.AddCustomAuth(builder.Configuration);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationExceptionFilter>();
}).AddApplicationPart(typeof(ControllersAssemblyMarker).Assembly);

builder.Services.AddCustomMediator();

builder.Services.AddCustomDb(builder.Configuration);

builder.Services.AddMessaging(builder.Configuration);

builder.Services.AddCustomHangfire(builder.Configuration);

builder.Services.AddCustomHealthChecks(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddRepositories();

var app = builder.Build();

app.ApplyMigrations();

app.UseCustomMiddlewares();
app.MapControllers();
app.UseCustomSwagger();
app.UseCustomHangfire();

app.Run();

// ReSharper disable once RedundantTypeDeclarationBody Трюк для использования Program в фикстурах
public partial class Program { } // For testing purposes