using AccountService.Core.Domain.Abstraction;
using AccountService.DatabaseAccess.Abstractions;

namespace AccountService.Application.Background;

public class AccrueInterestRateExecutor(IAccountRepository accountRepository, IUnitOfWork unitOfWork) : IAccrueInterestRateExecutor
{
	public async Task AccrueInterestRateAsync(Guid accountId, CancellationToken cancellationToken) 
	{
		await unitOfWork.BeginTransactionAsync(cancellationToken);

		try {
			var account = await accountRepository.GetByIdAsync(accountId, cancellationToken);
		}
		catch { }
	}
}