using AccountService.Core.Domain.Abstraction;
using FluentValidation;

namespace AccountService.Application.Features.Transactions.DeleteTransaction;

public class DeleteTransactionCommandValidator : AbstractValidator<DeleteTransactionCommand>
{
    public DeleteTransactionCommandValidator(ITransactionRepository transactionRepository)
    {
        RuleFor(x => x.TransactionId)
            .NotEmpty()
            .WithMessage("TransactionId is required.")
            .MustAsync(async (transactionId, cancellationToken) =>
                await transactionRepository.ExistsAsync(transactionId, cancellationToken));
    }
}