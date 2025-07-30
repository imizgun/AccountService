namespace AccountService.Application.Services.Abstractions;

public interface IClientService
{
    bool IsClientExists(Guid clientId);
    List<Guid> GetClientIds();
}