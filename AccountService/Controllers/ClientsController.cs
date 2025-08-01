using AccountService.Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientsController : ControllerBase
{
    private IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    /// <summary>
    /// Контроллер для получения списка ID пользователей для регистрации счетов
    /// </summary>
    /// <returns>Список ID пользователей</returns>
    /// <response code="200">Список пользователей</response>
    [HttpGet]
    public ActionResult<List<Guid>> GetClientIds()
    {
        var clientIds = _clientService.GetClientIds();
        return Ok(clientIds);
    }
}