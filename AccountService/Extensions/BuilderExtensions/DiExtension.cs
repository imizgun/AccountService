using AccountService.Application.Features.Accounts.DatabaseAccess;
using AccountService.Application.Features.Accounts.Domain;
using AccountService.Application.Features.Boxes.DatabaseAccess;
using AccountService.Application.Features.Boxes.Domain;
using AccountService.Application.Features.Interest.DatabaseAccess;
using AccountService.Application.Features.Transactions.DatabaseAccess;
using AccountService.Application.Features.Transactions.Domain;
using AccountService.Application.Shared.DatabaseAccess.Abstractions;
using AccountService.Application.Shared.DatabaseAccess.Repositories;
using AccountService.Application.Shared.Domain.Abstraction;
using AccountService.Application.Shared.Services.Abstractions;
using AccountService.Application.Shared.Services.Services;
using AccountService.Background.Rabbit.Background;

namespace AccountService.Extensions.BuilderExtensions;

public static class DiExtension
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWorkRepository>();
        services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();
        services.AddScoped<IInboxDeadLetterRepository, InboxDeadLetterRepository>();
        services.AddScoped<IInboxConsumedRepository, InboxConsumedRepository>();
        services.AddSingleton<ICurrencyService, CurrencyService>();
        services.AddScoped<IAccrueInterestRateExecutor, AccrueInterestRateExecutor>();
        services.AddScoped<IAccrueInterestRateSelector<Account>, AccrueInterestRateSelector>();
        services.AddHostedService<OutboxDispatcher>();
    }
}