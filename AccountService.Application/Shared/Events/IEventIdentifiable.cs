// ReSharper disable UnusedMemberInSuper.Global
namespace AccountService.Application.Shared.Events;


public interface IEventIdentifiable
{
    public Guid EventId { get; set; }
    public Meta Meta { get; set; }
    // ReSharper disable once UnusedMember.Global нужно при сохранении в БД и сериализации
    public DateTime OccurredAt { get; set; }
}