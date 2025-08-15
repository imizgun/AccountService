using AccountService.Application.Features.Accounts.Domain;
using AccountService.Application.Features.Accounts.Events;
using AccountService.Application.Features.Boxes.Domain;
using AccountService.Application.Shared.DatabaseAccess.Abstractions;
using AccountService.Application.Shared.Events;
using MediatR;

namespace AccountService.Application.Features.Accounts.Operations.DeleteAccount;

public class DeleteAccountCommandHandler(
    IAccountRepository accountRepository, 
    IUnitOfWork unitOfWork,
    IOutboxMessageRepository outboxMessageRepository)
    : IRequestHandler<DeleteAccountCommand, bool>
{
    public async Task<bool> Handle(DeleteAccountCommand request, CancellationToken cancellationToken) 
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try {
            var account = await accountRepository.GetByIdForUpdateAsync(request.AccountId, cancellationToken);

            if (account == null) throw new KeyNotFoundException("Account not found");

            if (account.ClosingDate != null) throw new InvalidOperationException("Account is already closed");

            account.Close();

            var closeAccountEvent = new AccountClosed(
                Guid.NewGuid(),
                DateTime.UtcNow,
                new Meta(request.CorrelationId),
                account.Id
            );

            await outboxMessageRepository.AddAsync(new OutboxMessage(closeAccountEvent), cancellationToken);
            var res = await unitOfWork.SaveChangesAsync(cancellationToken);
            
            if (res != 2) throw new InvalidOperationException("Error while closing account");

            await unitOfWork.CommitAsync(cancellationToken);
            return true;
        }
        catch {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}