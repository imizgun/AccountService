using AccountService.Core.Features.Transactions;
using FluentValidation;

namespace AccountService.Application.Features.Transactions.UpdateTransaction;

// ReSharper disable once UnusedMember.Global Валидатор неявно используется в Пайплайне
public class UpdateTransactionCommandValidator : AbstractValidator<UpdateTransactionCommand>
{
    public UpdateTransactionCommandValidator(ITransactionRepository transactionRepository)
    {
        RuleFor(x => x.TransactionId)
            .NotEmpty()
            .WithMessage("Transaction ID is required.")
            .MustAsync(async (id, cancellation) => await transactionRepository.ExistsAsync(id, cancellation));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters.");

    }
}