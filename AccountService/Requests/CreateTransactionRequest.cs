namespace AccountService.Requests;

public record CreateTransactionRequest(
    Guid? CounterpartyAccountId,
    string Currency,
    decimal Amount,
    string Description);