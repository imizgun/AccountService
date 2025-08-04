namespace AccountService.Application.Services.Abstractions;

public interface IClientService
{
    List<Guid> GetClientIds();
}