using AccountService.Application.Features.Accounts.CreateAccount;
using AccountService.Application.Features.Accounts.DeleteAccount;
using AccountService.Application.Features.DTOs;
using AccountService.Application.Features.Accounts.GetAccount;
using AccountService.Application.Features.Accounts.UpdateAccount;
using AccountService.Application.Features.Transactions.DeleteTransaction;
using AccountService.Application.Features.Transactions.GetTransactions;
using AccountService.Application.Features.Transactions.MakeTransactions;
using AccountService.Application.Features.Transactions.UpdateTransaction;
using AccountService.Requests;
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


    /// <summary>
    /// Возвращает список всех аккаунтов для клиента с ID OwnerId.
    /// </summary>
    /// <param name="ownerId">ID клиента</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Список всех клиентов</returns>
    /// <response code="200">Возвращает список аккаунтов</response>
    /// <response code="400">Некорректный запрос или ошибка на сервере</response>
    [HttpGet]
    public async Task<ActionResult<List<AccountDto>>> GetAccountsByOwner([FromQuery] Guid ownerId, CancellationToken cancellationToken)
    {
        var query = new GetAccountsByOwnerQuery(ownerId);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }


    /// <summary>
    /// Создает новый аккаунт для клиента
    /// </summary>
    /// <param name="command">Данные для создания счета</param>
    /// <param name="cancellationToken"></param>
    /// <returns>ID открытого счета</returns>
    /// <response code="200">Аккаунт успешно создан</response>
    /// <response code="400">Некорректный запрос или ошибка на сервере</response>
    [HttpPost]
    public async Task<ActionResult<CreateResponse>> CreateAccount([FromBody] CreateAccountCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        return result == Guid.Empty ? BadRequest(new CreateResponse(result)) : Ok(new CreateResponse(result));
    }


    /// <summary>
    /// Удаляет (закрывает) счёт по ID
    /// </summary>
    /// <param name="id">ID счёта</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Сообщение, характеризующее результат операции</returns>
    /// <response code="200">Счет успешно закрыт</response>
    /// <response code="400">Некорректный запрос или ошибка на сервере</response> 
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<SimpleResponse>> DeleteAccount(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteAccountCommand(id);
        var res = await _mediator.Send(command, cancellationToken);

        return res ?
            Ok(new SimpleResponse("Deleted successfully"))
            : NotFound(new SimpleResponse("Resource not found"));
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
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<SimpleResponse>> UpdateAccount(Guid id, [FromBody] UpdateAccountRequest command, CancellationToken cancellationToken)
    {
        var res = await _mediator.Send(new UpdateAccountCommand(id, command.InterestRate), cancellationToken);

        return res ?
                Ok(new SimpleResponse("Updated successfully"))
                : NotFound(new SimpleResponse("Resource not found"));
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
    [HttpGet("{id:guid}/transactions")]
    public async Task<ActionResult<List<TransactionDto>>> GetTransactionsByAccountId(
        Guid id,
        [FromQuery] int? skip,
        [FromQuery] int? take,
        CancellationToken cancellationToken)
    {
        var notNullSkip = skip ?? 0;
        var notNullTake = take ?? 10;

        var result = await _mediator.Send(new GetTransactionsCommand(id, notNullTake, notNullSkip), cancellationToken);

        return Ok(result);
    }


    /// <summary>
    /// Создает транзакцию со счета по ID, создает обратную транзакцию на счете контрагента, если указан
    /// </summary>
    /// <param name="id">ID счета</param>
    /// <param name="request">Тело запроса для счета</param>
    /// <param name="cancellationToken"></param>
    /// <returns>ID созданной транзакции</returns>
    /// <response code="200">Возвращает ID созданной транзакции</response>
    /// <response code="400">Некорректный запрос или ошибка на сервере</response>
    [HttpPost("{id:guid}/transactions")]
    public async Task<ActionResult<CreateResponse>> CreateTransaction(
        Guid id,
        [FromBody] CreateTransactionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new MakeTransactionCommand(id, request.CounterpartyAccountId, request.Currency, request.Amount, request.Description);
        var result = await _mediator.Send(command, cancellationToken);

        return result == Guid.Empty ? BadRequest(new CreateResponse(result)) : Ok(new CreateResponse(result));
    }


    /// <summary>
    /// Помечает транзакцию по ID как удаленную (не удаляет физически)
    /// </summary>
    /// <param name="id">ID счета</param>
    /// <param name="transactionId">ID транзакции</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Сообщение, успешно ли прошло удаление</returns>
    /// <response code="200">Успешное удаление</response>
    /// <response code="400">Некорректный запрос или ошибка на сервере</response>
    [HttpDelete("{id:guid}/transactions/{transactionId:guid}")]
    public async Task<ActionResult<SimpleResponse>> DeleteTransaction(Guid id, Guid transactionId, CancellationToken cancellationToken)
    {
        var command = new DeleteTransactionCommand(transactionId);
        var res = await _mediator.Send(command, cancellationToken);

        return res ?
            Ok(new SimpleResponse("Deleted successfully"))
            : NotFound(new SimpleResponse("Resource not found"));
    }

    /// <summary>
    /// Изменяет описание транзакции по ID (предполагается, что это делается для исправления ошибок)
    /// </summary>
    /// <param name="id">ID счета</param>
    /// <param name="transactionId">ID транзакции</param>
    /// <param name="cancellationToken"></param>
    /// <param name="request"></param>
    /// <returns>Сообщение, успешно ли прошло обновление</returns>
    /// <response code="200">Успешное обновление</response>
    /// <response code="400">Некорректный запрос или ошибка на сервере</response>
    [HttpPatch("{id:guid}/transactions/{transactionId:guid}")]
    public async Task<ActionResult<SimpleResponse>> UpdateTransaction(
        Guid id,
        Guid transactionId,
        [FromBody] UpdateTransactionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTransactionCommand(transactionId, request.Description);
        var res = await _mediator.Send(command, cancellationToken);

        return res ? BadRequest(new SimpleResponse("Update failed")) : Ok(new SimpleResponse("Updated successfully"));
    }
}