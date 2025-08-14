namespace AccountService.Features.Accounts;


/// <summary>
/// Запрос на создание счета
/// </summary>
/// <param name="Currency">Валюта</param>
/// <param name="AccountType">Тип аккаунта</param>
/// <param name="InterestRate">Процентная ставка (если соответствует тип счета)</param>
public record CreateAccountRequest(
    string Currency,
    string AccountType,
    decimal? InterestRate);