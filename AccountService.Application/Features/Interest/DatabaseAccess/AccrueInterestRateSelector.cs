using AccountService.Application.Features.Accounts.Domain;
using AccountService.Application.Shared.DatabaseAccess;
using AccountService.Application.Shared.Domain.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Application.Features.Interest.DatabaseAccess;

public class AccrueInterestRateSelector(AccountServiceDbContext dbContext) : IAccrueInterestRateSelector<Account>
{
    public async Task<List<Account>> SelectAccountsForAccrualAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Accounts
            .AsNoTracking()
            .Where(x => x.AccountType == AccountType.Credit || x.AccountType == AccountType.Deposit)
            .Where(x => x.ClosingDate == null)
            .ToListAsync(cancellationToken);
    }
}