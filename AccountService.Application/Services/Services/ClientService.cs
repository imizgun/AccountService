using AccountService.Application.Services.Abstractions;

namespace AccountService.Application.Services.Services;

public class ClientService : IClientService
{
    private List<Guid> _clients;

    public ClientService()
    {
        _clients = [
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid()
        ];
    }
    public bool IsClientExists(Guid clientId) => _clients.Contains(clientId);

    public List<Guid> GetClientIds() => _clients;
}