using AccountService.Application.Features.Transactions;
using AccountService.Application.Features.Transactions.Operations.GetTransactions;
using AccountService.Application.Shared.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Application.Features.Accounts;

[ApiController]
[Authorize]
[Route("/api/accounts")]
public partial class AccountController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Делает выписку со счета по ID
    /// </summary>
    /// <param name="id">ID счета</param>
    /// <param name="skip">Параметр пагинации (сколько пропустить)</param>
    /// <param name="take">Параметр пагинации (сколько взять)</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Список проведенных транзакций на счету</returns>
    /// <response code="200">Возвращает список транзакций</response>
    /// <response code="400">Некорректный запрос или ошибка на сервере</response>
    /// <response code="401">Необходима авторизация</response>
    [HttpGet("{id:guid}/transactions")]
    public async Task<ActionResult<MbResult<List<TransactionDto>>>> GetTransactionsByAccountId(
        Guid id,
        [FromQuery] int? skip,
        [FromQuery] int? take,
        CancellationToken cancellationToken)
    {
        var notNullSkip = skip ?? 0;
        var notNullTake = take ?? 10;

        var result = await mediator.Send(new GetTransactionsCommand(id, notNullTake, notNullSkip), cancellationToken);

        return Ok(MbResult<List<TransactionDto>>.Ok(result));
    }
}