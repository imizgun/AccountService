namespace AccountService.Application.Services.Abstractions;

public interface ICurrencyService
{
    bool IsValidCurrency(string currencyCode);
}