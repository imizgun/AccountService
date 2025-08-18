// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength Длина строки ограничена названием типа, а содержимое JSON, длина может быть большой
namespace AccountService.Application.Features.Boxes.Domain;

public class InboxDeadLetter
{
    public Guid MessageId { get; set; }
    public DateTime ReceivedAt { get; set; }
    public string Handler { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
}