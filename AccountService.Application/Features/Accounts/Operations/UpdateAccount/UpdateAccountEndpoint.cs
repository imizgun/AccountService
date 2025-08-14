using AccountService.Application.Features.Accounts.Operations.UpdateAccount;
using AccountService.Application.Shared.Contracts;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable once CheckNamespace Это частичный класс и у него должен быть тот же неймспейс

namespace AccountService.Application.Features.Accounts;

public partial class AccountController
{
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
}