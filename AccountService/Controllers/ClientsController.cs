using AccountService.Application.Services.Abstractions;
using AccountService.Responses;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientsController(IClientService clientService) : ControllerBase
{

    /// <summary>
    /// Контроллер для получения списка ID пользователей для регистрации счетов (устарел и на момент сдачи 2-го задания не несет особой функциональности 
    /// </summary>
    /// <returns>Список ID пользователей</returns>
    /// <response code="200">Список пользователей</response>
    [HttpGet]
    public ActionResult<MbResult<List<Guid>>> GetClientIds()
    {
        var clientIds = clientService.GetClientIds();
        return Ok(MbResult<List<Guid>>.Ok(clientIds));
    }
}