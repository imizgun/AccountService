using MediatR;

namespace AccountService.Application.Features.Accounts.Operations.CreateAccount;

/// <summary>
///   Команда для создания аккаунта
/// </summary>
/// <param name="OwnerId">ID юзера</param>
/// <param name="Currency">Валюта</param>
/// <param name="AccountType">Тип аккаунта</param>
/// <param name="InterestRate">Процентная ставка</param>
public record CreateAccountCommand(
    Guid OwnerId,
    string Currency,
    string AccountType,
    decimal? InterestRate) : IRequest<Guid>;