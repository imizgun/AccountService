namespace AccountService.Application.Features.Boxes.Domain;

public class InboxConsumed
{
    public Guid MessageId { get; set; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength Это имя хэнделра, так что оно берется из имени типаы
    public string Handler { get; set; } = "";
    public DateTime ProcessedAt { get; set; }
}