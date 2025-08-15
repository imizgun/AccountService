namespace AccountService.Application.Features.Accounts.Operations.UpdateAccount;


/// <summary>
/// Запрос на изменение процентной ставки (для всех счетов, кроме Checking)
/// </summary>
/// <param name="InterestRate">Новая процентная ставка</param>
public record UpdateAccountRequest(decimal InterestRate);