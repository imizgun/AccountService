namespace AccountService.Application.Shared.Events;

public interface IEventIdentifiable 
{
	public Guid EventId { get; set; }	
	public Meta Meta { get; set; }
	public DateTime OccurredAt { get; set; }
}