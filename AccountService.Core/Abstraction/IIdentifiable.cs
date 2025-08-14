namespace AccountService.Core.Abstraction;

public interface IIdentifiable
{
    public Guid Id { get; set; }
}