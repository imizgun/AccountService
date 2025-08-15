namespace AccountService.Application.Features.Boxes.Domain;

public class InboxConsumed
{
	public Guid MessageId { get; set; }
	public string Handler { get; set; } = "";
	public DateTime ProcessedAt { get; set; }
}