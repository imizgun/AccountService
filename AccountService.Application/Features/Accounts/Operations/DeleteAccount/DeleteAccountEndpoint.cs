using AccountService.Application.Features.Accounts.Operations.DeleteAccount;
using AccountService.Application.Shared.Contracts;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable once CheckNamespace Это частичный класс и у него должен быть тот же неймспейс

namespace AccountService.Application.Features.Accounts;

public partial class AccountController
{
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
        var command = new DeleteAccountCommand(id, Guid.NewGuid());
        await mediator.Send(command, cancellationToken);

        return Ok(MbResult<Guid>.Ok(id, "Deleted successfully"));
    }
}