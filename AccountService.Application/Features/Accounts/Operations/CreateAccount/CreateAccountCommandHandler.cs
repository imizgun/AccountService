using AccountService.Application.Features.Accounts.Domain;
using AccountService.Application.Features.Accounts.Events;
using AccountService.Application.Features.Boxes.Domain;
using AccountService.Application.Shared.DatabaseAccess.Abstractions;
using AccountService.Application.Shared.Events;
using MediatR;

namespace AccountService.Application.Features.Accounts.Operations.CreateAccount;

public class CreateAccountCommandHandler(
    IAccountRepository accountRepository, 
    IUnitOfWork unitOfWork, 
    IOutboxMessageRepository outboxMessageRepository)
    : IRequestHandler<CreateAccountCommand, Guid>
{
    public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var eventId = Guid.NewGuid();
        if (!Enum.TryParse<AccountType>(request.AccountType, true, out var type))
            throw new ArgumentException("Invalid account type");

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try {
            var newAccount = Account.Create(
                request.OwnerId,
                type,
                request.Currency,
                request.InterestRate
            );

            var res = await accountRepository.CreateAsync(newAccount, cancellationToken);
        
            var openAccountEvent = new AccountOpened(
                eventId, 
                DateTime.UtcNow, 
                new Meta(request.CorrelationId),
                newAccount.Id,
                newAccount.OwnerId,
                newAccount.Currency,
                newAccount.AccountType.ToString()
            );
        
            await outboxMessageRepository.AddAsync(new OutboxMessage(openAccountEvent), cancellationToken);
            var save = await unitOfWork.SaveChangesAsync(cancellationToken);

            if (save != 2) throw new InvalidOperationException("Failed to create account");
            
            await unitOfWork.CommitAsync(cancellationToken);
        
            return res;
        }
        catch {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
        
    }
}