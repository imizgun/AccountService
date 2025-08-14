using MediatR;

namespace AccountService.Application.Features.Accounts.Operations.DeleteAccount;

public record DeleteAccountCommand(Guid AccountId) : IRequest<bool>;