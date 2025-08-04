using AccountService.Application.Services.Abstractions;

namespace AccountService.Application.Services.Services;

public class ClientService : IClientService
{
    private readonly List<Guid> _clients = [
        Guid.NewGuid(),
        Guid.NewGuid(),
        Guid.NewGuid()
    ];

    public List<Guid> GetClientIds() => _clients;
}