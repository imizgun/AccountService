using System.Globalization;
using AccountService.Application.Shared.Services.Abstractions;

namespace AccountService.Application.Shared.Services.Services;

public class CurrencyService : ICurrencyService
{
    private readonly HashSet<string> _currencies = [];

    public CurrencyService()
    {
        foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
        {
            var r = new RegionInfo(culture.Name);
            _currencies.Add(r.ISOCurrencySymbol);
        }
    }
    public bool IsValidCurrency(string currencyCode)
    {
        return _currencies.Contains(currencyCode.ToUpper());
    }
}