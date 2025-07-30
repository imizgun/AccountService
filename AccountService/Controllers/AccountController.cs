using AccountService.Application.Features.Accounts.CreateAccount;
using AccountService.Application.Features.Accounts.DeleteAccount;
using AccountService.Application.Features.Accounts.DTOs;
using AccountService.Application.Features.Accounts.GetAccount;
using AccountService.Application.Features.Accounts.UpdateAccount;
using AccountService.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Controllers;

[ApiController]
[Route("/api/accounts")]
public class AccountController : ControllerBase
{
    private IMediator _mediator;

    public AccountController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<AccountDto>>> GetAccountsByOwner([FromQuery] Guid ownerId, CancellationToken cancellationToken)
    {
        var query = new GetAccountsByOwnerQuery(ownerId);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<CreateResponse>> CreateAccount([FromBody] CreateAccountCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        return result == Guid.Empty ? BadRequest(new CreateResponse(result)) : Ok(new CreateResponse(result));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<bool>> DeleteAccount(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteAccountCommand(id);
        var res = await _mediator.Send(command, cancellationToken);

        return res ?
            Ok(new SimpleResponse("Deleted successfully"))
            : NotFound(new SimpleResponse("Resource not found"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SimpleResponse>> UpdateAccount(Guid id, [FromBody] UpdateAccountCommand command, CancellationToken cancellationToken)
    {
        command = command with { AccountId = id };
        var res = await _mediator.Send(command, cancellationToken);

        return res ?
                Ok(new SimpleResponse("Updated successfully"))
                : NotFound(new SimpleResponse("Resource not found"));
    }
}