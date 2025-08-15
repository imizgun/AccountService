using AccountService.Application.Features.Accounts.Domain;
using AccountService.Application.Features.Accounts.Events;
using AccountService.Application.Features.Boxes.Domain;
using AccountService.Application.Shared.DatabaseAccess.Abstractions;
using AccountService.Application.Shared.Events;
using MediatR;

namespace AccountService.Application.Features.Accounts.Operations.UpdateAccount;

public class UpdateAccountCommandHandler(
    IAccountRepository accountRepository, 
    IUnitOfWork unitOfWork, 
    IOutboxMessageRepository outboxMessageRepository)
    : IRequestHandler<UpdateAccountCommand, bool>
{
    public async Task<bool> Handle(UpdateAccountCommand request, CancellationToken cancellationToken) 
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try {
            var account = await accountRepository.GetByIdForUpdateAsync(request.AccountId, cancellationToken);

            if (account == null) throw new InvalidOperationException("Account is already closed");

            account.InterestRate = request.InterestRate;
            var updateAccountEvent = new AccountUpdated(
                Guid.NewGuid(),
                DateTime.UtcNow,
                new Meta(request.CorrelationId),
                account.Id,
                request.InterestRate
            );

            await outboxMessageRepository.AddAsync(new OutboxMessage(updateAccountEvent), cancellationToken);

            var res = await unitOfWork.SaveChangesAsync(cancellationToken);
            
            return res == 2 ? true : throw new InvalidOperationException("Error while updating account");;
        }
        catch {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}