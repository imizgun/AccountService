using AccountService;
using AccountService.Application.Features.Accounts.DatabaseAccess;
using AccountService.Application.Features.Accounts.Domain;
using AccountService.Application.Features.Accounts.Operations.CreateAccount;
using AccountService.Application.Features.Boxes.DatabaseAccess;
using AccountService.Application.Features.Boxes.Domain;
using AccountService.Application.Features.Interest.DatabaseAccess;
using AccountService.Application.Features.Transactions.DatabaseAccess;
using AccountService.Application.Features.Transactions.Domain;
using AccountService.Application.Shared;
using AccountService.Application.Shared.Behaviors;
using AccountService.Application.Shared.DatabaseAccess;
using AccountService.Application.Shared.DatabaseAccess.Abstractions;
using AccountService.Application.Shared.DatabaseAccess.Repositories;
using AccountService.Application.Shared.Domain.Abstraction;
using AccountService.Application.Shared.Services.Abstractions;
using AccountService.Application.Shared.Services.Services;
using AccountService.Background.DailyAccrueInterestRate;
using AccountService.Background.Rabbit;
using AccountService.Background.Rabbit.Background;
using AccountService.Configs;
using AccountService.Filters;
using AccountService.Middlewares;
using FluentValidation;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, services, cfg) =>
    cfg.ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

builder.Services.AddHttpLogging(logging => {
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
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
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
            []
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["JwtSettings:Authority"];
        options.Audience = builder.Configuration["JwtSettings:Audience"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationExceptionFilter>();
})
    .AddApplicationPart(typeof(ControllersAssemblyMarker).Assembly);

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(typeof(CreateAccountCommandValidator).Assembly);
});

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MapperProfile>());

builder.Services.AddDbContext<AccountServiceDbContext>(opt =>
{
    var cs = builder.Configuration.GetConnectionString(nameof(AccountServiceDbContext));
    opt.UseNpgsql(cs);
});

builder.Services.AddMessaging(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWorkRepository>();
builder.Services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();
builder.Services.AddScoped<IInboxDeadLetterRepository, InboxDeadLetterRepository>();
builder.Services.AddScoped<IInboxConsumedRepository, InboxConsumedRepository>();
builder.Services.AddSingleton<ICurrencyService, CurrencyService>();

ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
builder.Services.AddValidatorsFromAssemblyContaining<CreateAccountCommandValidator>();
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddScoped<IAccrueInterestRateExecutor, AccrueInterestRateExecutor>();
builder.Services.AddScoped<IAccrueInterestRateSelector<Account>, AccrueInterestRateSelector>();
builder.Services.AddScoped<AccrueInterestRateJob>();
builder.Services.AddHangfireWithPostgres(builder.Configuration);
builder.Services.AddHostedService<OutboxDispatcher>();

var app = builder.Build();

app.UseHttpLogging();
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            
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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AccountServiceDbContext>();
    db.Database.Migrate();
}

app.UseExceptionHandler();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = [new AllowAllDashboardAuthorizationFilter()]
});
app.AddDailyInterestRecurringJob(TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));

app.UseCors(c => c
    .AllowAnyHeader()
    .AllowAnyOrigin()
    .AllowAnyMethod()
    );

app.UseHttpsRedirection();
app.Run();

// ReSharper disable once RedundantTypeDeclarationBody Трюк для использования Program в фикстурах
public partial class Program { } // For testing purposes