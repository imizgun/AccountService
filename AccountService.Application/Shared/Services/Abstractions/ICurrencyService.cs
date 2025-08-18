namespace AccountService.Application.Shared.Services.Abstractions;

public interface ICurrencyService
{
    bool IsValidCurrency(string currencyCode);
}