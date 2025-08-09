using MediatR;

namespace AccountService.Application.Features.Accounts.DeleteAccount;
// ReSharper disable once IdentifierTypo
public record DeleteAccountCommand(Guid AccountId, uint Xmin) : IRequest<bool>;