using AccountService;
using AccountService.Application.Behaviors;
using AccountService.Application.Features.Accounts.CreateAccount;
using AccountService.Application.Services.Abstractions;
using AccountService.Application.Services.Services;
using AccountService.Core.Domain.Abstraction;
using AccountService.DatabaseAccess;
using AccountService.DatabaseAccess.Abstractions;
using AccountService.DatabaseAccess.Repositories;
using AccountService.Filters;
using AccountService.Responses;
using AccountService.Utils;
using FluentValidation;
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
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(typeof(CreateAccountCommandValidator).Assembly);
});

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MapperProfile>());

builder.Services.AddDbContext<AccountServiceDbContext>(opt => {
    var cs = builder.Configuration.GetConnectionString(nameof(AccountServiceDbContext));
    opt.UseNpgsql(cs);
});

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWorkRepository>();
builder.Services.AddSingleton<ICurrencyService, CurrencyService>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateAccountCommandValidator>();
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

var app = builder.Build();

using (var scope = app.Services.CreateScope()) 
{
    var db = scope.ServiceProvider.GetRequiredService<AccountServiceDbContext>();
    db.Database.Migrate();
}

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (DbUpdateConcurrencyException)
    {
        context.Response.StatusCode = StatusCodes.Status409Conflict;
        await context.Response.WriteAsJsonAsync(MbResult<object>.Fail("Concurrency conflict"));
    }
    catch (DbUpdateException ex) when (Utils.TryGetPg(ex, out var pg) && (pg.SqlState == "40001" || pg.SqlState == "40P01"))
    {
        context.Response.StatusCode = StatusCodes.Status409Conflict;
        await context.Response.WriteAsJsonAsync(MbResult<object>.Fail($"DB conflict ({pg.SqlState})"));
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("transient failure", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.StatusCode = StatusCodes.Status409Conflict;
        await context.Response.WriteAsJsonAsync(MbResult<object>.Fail("Transient failure, please retry"));
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(MbResult<object>.Fail(ex.Message));
    }
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(c => c
    .AllowAnyHeader()
    .AllowAnyOrigin()
    .AllowAnyMethod()
    );

app.UseHttpsRedirection();
app.Run();

public partial class Program { } // For testing purposes