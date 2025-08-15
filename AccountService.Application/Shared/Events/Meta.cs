namespace AccountService.Application.Shared.Events;

public class Meta(Guid correlationId) {
	public string Version { get; set; } = "v1";
	public string Source { get; set; } = "AccountService";
	public Guid CorrelationId { get; set; } = correlationId;
	public Guid CausationId { get; set; } = Guid.NewGuid();
}