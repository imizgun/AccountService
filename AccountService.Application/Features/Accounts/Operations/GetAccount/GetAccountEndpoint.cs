using System.Security.Claims;
using AccountService.Application.Features.Accounts.Operations.GetAccount;
using AccountService.Application.Shared.Contracts;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable once CheckNamespace Это частичный класс и у него должен быть тот же неймспейс

namespace AccountService.Application.Features.Accounts;

public partial class AccountController
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

}