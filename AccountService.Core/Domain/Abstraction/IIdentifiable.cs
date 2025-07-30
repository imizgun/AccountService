namespace AccountService.Core.Domain.Abstraction;

public interface IIdentifiable
{
    public Guid Id { get; set; }
}