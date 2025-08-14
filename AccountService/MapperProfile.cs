using AccountService.Application.Features.Accounts;
using AccountService.Application.Features.Accounts.Domain;
using AccountService.Application.Features.Transactions;
using AccountService.Application.Features.Transactions.Domain;
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