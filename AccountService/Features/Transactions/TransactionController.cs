using AccountService.Application.Features.Transactions.DeleteTransaction;
using AccountService.Application.Features.Transactions.MakeTransactions;
using AccountService.Application.Features.Transactions.UpdateTransaction;
using AccountService.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.Transactions;

[ApiController]
[Authorize]
[Route("/api/transactions")]
public class TransactionController(IMediator mediator) : ControllerBase
{
	/// <summary>
    /// Создает транзакцию со счета по ID, создает обратную транзакцию на счете контрагента, если указан
    /// </summary>
    /// <param name="request">Тело запроса для счета</param>
    /// <param name="cancellationToken"></param>
    /// <returns>ID созданной транзакции</returns>
    /// <response code="200">Возвращает ID созданной транзакции</response>
    /// <response code="400">Некорректный запрос или ошибка на сервере</response>
    /// <response code="401">Необходима авторизация</response>
    [HttpPost]
    public async Task<ActionResult<MbResult<Guid>>> CreateTransaction(
        [FromBody] MakeTransactionCommand request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);

        return Ok(MbResult<Guid>.Ok(result, "Transaction created successfully"));
    }

    // ReSharper disable once CommentTypo
    /// <summary>
    /// Помечает транзакцию по ID как удаленную (не удаляет физически)
    /// </summary>
    /// <param name="transactionId">ID транзакции</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Сообщение, успешно ли прошло удаление</returns>
    /// <response code="200">Успешное удаление</response>
    /// <response code="400">Некорректный запрос или ошибка на сервере</response>
    /// <response code="401">Необходима авторизация</response>
    [HttpDelete("{transactionId:guid}")]
    public async Task<ActionResult<MbResult<Guid>>> DeleteTransaction(Guid transactionId, CancellationToken cancellationToken)
    {
        var command = new DeleteTransactionCommand(transactionId);
        await mediator.Send(command, cancellationToken);

        return Ok(MbResult<Guid>.Ok(transactionId, "Deleted successfully"));
    }

    /// <summary>
    /// Изменяет описание транзакции по ID (предполагается, что это делается для исправления ошибок)
    /// </summary>
    /// <param name="transactionId">ID транзакции</param>
    /// <param name="cancellationToken"></param>
    /// <param name="request"></param>
    /// <returns>Сообщение, успешно ли прошло обновление</returns>
    /// <response code="200">Успешное обновление</response>
    /// <response code="400">Некорректный запрос или ошибка на сервере</response>
    /// <response code="401">Необходима авторизация</response>
    [HttpPatch("{transactionId:guid}")]
    public async Task<ActionResult<MbResult<Guid>>> UpdateTransaction(
        Guid transactionId,
        [FromBody] UpdateTransactionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTransactionCommand(transactionId, request.Description);
        await mediator.Send(command, cancellationToken);

        return Ok(MbResult<Guid>.Ok(transactionId, "Updated successfully"));
    }
}