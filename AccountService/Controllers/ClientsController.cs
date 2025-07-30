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

    [HttpGet]
    public ActionResult<List<Guid>> GetClientIds()
    {
        var clientIds = _clientService.GetClientIds();
        return Ok(clientIds);
    }
}