using System.Security.Claims;
using AccountService.Application.Features.Accounts.Operations.CreateAccount;
using AccountService.Application.Shared.Contracts;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable once CheckNamespace Это частичный класс и у него должен быть тот же неймспейс

namespace AccountService.Application.Features.Accounts;

public partial class AccountController
{
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
                request.InterestRate,
                Guid.NewGuid()
            );

        var result = await mediator.Send(command, cancellationToken);

        return Ok(MbResult<Guid>.Ok(result));
    }
}