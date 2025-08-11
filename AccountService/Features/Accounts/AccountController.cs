using System.Security.Claims;
using AccountService.Application.Features.Accounts;
using AccountService.Application.Features.Accounts.CreateAccount;
using AccountService.Application.Features.Accounts.DeleteAccount;
using AccountService.Application.Features.Accounts.GetAccount;
using AccountService.Application.Features.Accounts.UpdateAccount;
using AccountService.Application.Features.Transactions;
using AccountService.Application.Features.Transactions.GetTransactions;
using AccountService.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.Accounts;

[ApiController]
[Authorize]
[Route("/api/accounts")]
public class AccountController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Возвращает список всех аккаунтов для клиента с ID OwnerId.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Список всех клиентов</returns>
    /// <response code="200">Возвращает список аккаунтов</response>
    /// <response code="400">Некорректный запрос или ошибка на сервере</response>
    /// <response code="401">Необходима авторизация</response>
    [HttpGet]
    public async Task<ActionResult<MbResult<List<AccountDto>>>> GetAccountsByOwner(CancellationToken cancellationToken)
    {
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(sub, out var ownerId))
            return BadRequest(MbResult<List<AccountDto>>.Fail("Invalid user ID format"));

        var query = new GetAccountsByOwnerQuery(ownerId);
        var result = await mediator.Send(query, cancellationToken);

        return Ok(MbResult<List<AccountDto>>.Ok(result));
    }



    /// <summary>
    /// Создает новый аккаунт для клиента
    /// </summary>
    /// <param name="request">Данные для создания счета</param>
    /// <param name="cancellationToken"></param>
    /// <returns>ID открытого счета</returns>
    /// <response code="200">Аккаунт успешно создан</response>
    /// <response code="400">Некорректный запрос или ошибка на сервере</response>
    /// <response code="401">Необходима авторизация</response>
    [HttpPost]
    public async Task<ActionResult<MbResult<Guid>>> CreateAccount([FromBody] CreateAccountRequest request, CancellationToken cancellationToken)
    {
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(sub, out var ownerId))
            return BadRequest(MbResult<List<AccountDto>>.Fail("Invalid user ID format"));

        var command = new CreateAccountCommand(
                ownerId,
                request.Currency,
                request.AccountType,
                request.InterestRate
            );

        var result = await mediator.Send(command, cancellationToken);

        return Ok(MbResult<Guid>.Ok(result));
    }


    /// <summary>
    /// Удаляет (закрывает) счёт по ID
    /// </summary>
    /// <param name="id">ID счёта</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Сообщение, характеризующее результат операции</returns>
    /// <response code="200">Счет успешно закрыт</response>
    /// <response code="400">Некорректный запрос или ошибка на сервере</response> 
    /// <response code="401">Необходима авторизация</response>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<MbResult<Guid>>> DeleteAccount(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteAccountCommand(id);
        await mediator.Send(command, cancellationToken);

        return Ok(MbResult<Guid>.Ok(id, "Deleted successfully"));
    }

    /// <summary>
    /// Изменяет процентную ставку по счёту
    /// </summary>
    /// <param name="id">ID счета</param>
    /// <param name="command">Новая процентная ставка</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Сообщение, характеризующее результат операции</returns>
    /// <response code="200">Счет успешно обновлен</response>
    /// <response code="400">Некорректный запрос или ошибка на сервере</response>
    /// <response code="401">Необходима авторизация</response>
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<MbResult<Guid>>> UpdateAccount(Guid id, [FromBody] UpdateAccountRequest command, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateAccountCommand(id, command.InterestRate), cancellationToken);

        return Ok(MbResult<Guid>.Ok(id, "Updated successfully"));
    }

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