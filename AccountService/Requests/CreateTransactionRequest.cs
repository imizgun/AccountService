namespace AccountService.Requests;

public record CreateTransactionRequest(
    Guid? CounterpartyAccountId,
    string TransactionType,
    string Currency,
    decimal Amount,
    string Description);