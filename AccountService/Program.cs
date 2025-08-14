using AccountService;
using AccountService.Application.Features.Accounts.DatabaseAccess;
using AccountService.Application.Features.Accounts.Domain;
using AccountService.Application.Features.Accounts.Operations.CreateAccount;
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
using AccountService.Configs;
using AccountService.Filters;
using AccountService.Middlewares;
using FluentValidation;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWorkRepository>();
builder.Services.AddSingleton<ICurrencyService, CurrencyService>();

ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
builder.Services.AddValidatorsFromAssemblyContaining<CreateAccountCommandValidator>();
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddScoped<IAccrueInterestRateExecutor, AccrueInterestRateExecutor>();
builder.Services.AddScoped<IAccrueInterestRateSelector, AccrueInterestRateSelector>();
builder.Services.AddScoped<AccrueInterestRateJob>();
builder.Services.AddHangfireWithPostgres(builder.Configuration);

var app = builder.Build();

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