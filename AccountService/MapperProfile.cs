using AccountService.Application.Features.DTOs;
using AccountService.Core.Domain.Entities;
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