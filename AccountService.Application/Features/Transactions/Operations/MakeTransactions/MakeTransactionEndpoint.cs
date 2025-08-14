using AccountService.Application.Features.Transactions.Operations.MakeTransactions;
using AccountService.Application.Shared.Contracts;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable once CheckNamespace Это частичный класс и у него должен быть тот же неймспейс

namespace AccountService.Application.Features.Transactions;

public partial class TransactionController
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
}