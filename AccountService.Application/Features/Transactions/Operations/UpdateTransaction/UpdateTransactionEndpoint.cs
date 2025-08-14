using AccountService.Application.Features.Transactions.Operations.UpdateTransaction;
using AccountService.Application.Shared.Contracts;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable once CheckNamespace Это частичный класс и у него должен быть тот же неймспейс

namespace AccountService.Application.Features.Transactions;

public partial class TransactionController
{
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