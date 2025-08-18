using AccountService.Application.Features.Boxes.Domain;
using AccountService.Application.Shared.DatabaseAccess;

namespace AccountService.Application.Features.Boxes.DatabaseAccess;

public class InboxDeadLetterRepository(AccountServiceDbContext dbContext) : IInboxDeadLetterRepository
{
    public async Task AddAsync(InboxDeadLetter deadLetter)
    {
        await dbContext.InboxDeadLetters.AddAsync(deadLetter);
    }
}