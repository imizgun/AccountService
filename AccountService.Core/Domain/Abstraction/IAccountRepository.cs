﻿using AccountService.Core.Domain.Entities;

namespace AccountService.Core.Domain.Abstraction;

public interface IAccountRepository : IBaseRepository<Account>
{
    // TODO: сделать метод получения только счетов, без транзакций
    Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Account>> GetAllOwnerAccounts(Guid ownerId, CancellationToken cancellationToken);
    Task<bool> CloseAccountAsync(Account account, CancellationToken cancellationToken);
    Task<bool> UpdateAccount(Account account, CancellationToken cancellationToken);
}