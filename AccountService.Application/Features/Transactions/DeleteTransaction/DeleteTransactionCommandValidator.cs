using AccountService.Core.Features.Transactions;
using FluentValidation;

namespace AccountService.Application.Features.Transactions.DeleteTransaction;

// ReSharper disable once UnusedMember.Global Валидатор неявно используется в Пайплайне
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