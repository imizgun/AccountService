using MediatR;

namespace AccountService.Application.Features.Accounts.DeleteAccount;
public record DeleteAccountCommand(Guid AccountId) : IRequest<bool>;