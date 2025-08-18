using AccountService.Application.Features.Accounts.Operations.CreateAccount;
using AccountService.Application.Shared.Behaviors;
using FluentValidation;
using MediatR;

namespace AccountService.Extensions.BuilderExtensions;

public static class MediatorExtensions
{
    public static void AddCustomMediator(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateAccountCommandValidator).Assembly));
        services.AddAutoMapper(cfg => cfg.AddProfile<MapperProfile>());
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        services.AddValidatorsFromAssemblyContaining<CreateAccountCommandValidator>();
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    }
}