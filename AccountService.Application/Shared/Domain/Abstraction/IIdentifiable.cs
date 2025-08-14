namespace AccountService.Application.Shared.Domain.Abstraction;

public interface IIdentifiable
{
    public Guid Id { get; set; }
}