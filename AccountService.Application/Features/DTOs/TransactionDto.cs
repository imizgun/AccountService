using AccountService.Core.Domain.Enums;

namespace AccountService.Application.Features.Accounts.DTOs;

public class TransactionDto
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Guid? CounterpartyAccountId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = null!;
    public TransactionType TransactionType { get; set; }
    public string Description { get; set; } = null!;
    public DateTime TransactionDate { get; set; }
}