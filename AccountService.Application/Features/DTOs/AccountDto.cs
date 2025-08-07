// ReSharper disable UnusedMember.Global Якобы ненужные, по мнению линтера, поля необходимы для представления сущности и используются в маппинге
namespace AccountService.Application.Features.DTOs;


/// <summary>
/// DTO для представления аккаунта
/// </summary>
public class AccountDto
{
    /// <summary>
    /// ID счета
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID владельца счета
    /// </summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// Тип аккаунта
    /// </summary>
    public string AccountType { get; set; } = null!;

    /// <summary>
    /// Валюта
    /// </summary>
    public string Currency { get; set; } = null!;

    /// <summary>
    /// Текущий баланс
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// Процентная ставка (опционально)
    /// </summary>
    public decimal? InterestRate { get; set; }

    /// <summary>
    /// Дата открытия
    /// </summary>
    public DateTime OpeningDate { get; set; }

    /// <summary>
    /// Дата закрытия (nullable)
    /// </summary>
    public DateTime? ClosingDate { get; set; }

    // public List<TransactionDto> Transactions { get; set; } = new();
}