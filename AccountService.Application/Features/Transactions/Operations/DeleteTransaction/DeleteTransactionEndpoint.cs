using AccountService.Application.Features.Transactions.Operations.DeleteTransaction;
using AccountService.Application.Shared.Contracts;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable once CheckNamespace Это частичный класс и у него должен быть тот же неймспейс

namespace AccountService.Application.Features.Transactions;

public partial class TransactionController
{
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
}