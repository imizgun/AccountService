namespace AccountService.Application.Features.DTOs;

public class TransactionDto
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Guid? CounterpartyAccountId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = null!;
    public string TransactionType { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime TransactionDate { get; set; }
}