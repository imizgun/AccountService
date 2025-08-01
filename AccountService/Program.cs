using AccountService;
using AccountService.Application.Behaviors;
using AccountService.Application.Features.Accounts.CreateAccount;
using AccountService.Application.Services.Abstractions;
using AccountService.Application.Services.Services;
using AccountService.Core.Domain.Abstraction;
using AccountService.DatabaseAccess.Repositories;
using AccountService.Filters;
using AccountService.Responses;
using FluentValidation;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var mainXml = Path.Combine(AppContext.BaseDirectory, "AccountService.xml");
    var appXml = Path.Combine(AppContext.BaseDirectory, "AccountService.Application.xml");

    c.IncludeXmlComments(mainXml);
    c.IncludeXmlComments(appXml);
});


builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationExceptionFilter>();
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(typeof(CreateAccountCommandValidator).Assembly);
});

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MapperProfile>());

builder.Services.AddSingleton<IAccountRepository, AccountRepository>();
builder.Services.AddSingleton<ITransactionRepository, TransactionRepository>();
builder.Services.AddSingleton<IClientService, ClientService>();
builder.Services.AddSingleton<ICurrencyService, CurrencyService>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateAccountCommandValidator>();
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

var app = builder.Build();

app.Use(async (context, next) =>
{
    try
    {
        await next.Invoke();
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsJsonAsync(MbResult<object>.Fail(ex.Message));
    }
});

app.UseRouting();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(c => c
    .AllowAnyHeader()
    .AllowAnyOrigin()
    .AllowAnyMethod()
    );

app.UseHttpsRedirection();
app.Run();