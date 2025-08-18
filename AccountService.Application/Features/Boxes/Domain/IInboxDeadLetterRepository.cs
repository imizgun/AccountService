namespace AccountService.Application.Features.Boxes.Domain;

public interface IInboxDeadLetterRepository
{
    Task AddAsync(InboxDeadLetter deadLetter);
}