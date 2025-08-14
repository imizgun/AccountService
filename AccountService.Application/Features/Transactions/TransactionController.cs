using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Application.Features.Transactions;

[ApiController]
[Authorize]
[Route("/api/transactions")]
public partial class TransactionController(IMediator mediator) : ControllerBase;