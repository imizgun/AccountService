using AccountService.Core.Abstraction;
using AccountService.Core.Features.Accounts;
using Microsoft.EntityFrameworkCore;

namespace AccountService.DatabaseAccess.Features.Background;

public class AccrueInterestRateSelector(AccountServiceDbContext dbContext) : IAccrueInterestRateSelector 
{
	public async Task<List<Guid>> SelectAccountsForAccrualAsync(CancellationToken cancellationToken) 
	{
		return await dbContext.Accounts
			.AsNoTracking()
			.Where(x => x.AccountType == AccountType.Credit || x.AccountType == AccountType.Deposit)
			.Where(x => x.ClosingDate == null)
			.Select(x => x.Id)
			.ToListAsync(cancellationToken);
	}
}