using AccountService.Core.Domain.Entities;
using AccountService.Core.Domain.Enums;

namespace AccountService.Application.Features.Accounts.DTOs;

public class AccountDto {
	public Guid Id { get; set; }
	public Guid OwnerId { get; set; }
	public string AccountType { get; set; } = null!;
	public string Currency { get; set; } = null!;
	public decimal Balance { get; set; }
	public decimal? InterestRate { get; set; }
	public DateTime OpeningDate { get; set; }
	public DateTime? ClosingDate { get; set; }

	public List<Transaction> Transactions { get; set; } = new();
}