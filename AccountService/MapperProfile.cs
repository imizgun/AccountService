using AccountService.Application.Features.Accounts;
using AccountService.Application.Features.Transactions;
using AccountService.Core.Features.Accounts;
using AccountService.Core.Features.Transactions;
using AutoMapper;

namespace AccountService;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Account, AccountDto>()
            .PreserveReferences()
            .MaxDepth(3);

        CreateMap<Transaction, TransactionDto>()
            .PreserveReferences()
            .MaxDepth(3);
    }
}