// ReSharper disable UnusedMember.Global Эти поля описывают сущность и участвуют в маппинге
namespace AccountService.Application.DTOs;


/// <summary>
/// DTO для транзакции
/// </summary>
public class TransactionDto
{
    /// <summary>
    /// ID транзакции
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID счета 
    /// </summary>
    public Guid AccountId { get; set; }

    /// <summary>
    /// ID контрагента (если есть)
    /// </summary>
    public Guid? CounterpartyAccountId { get; set; }

    /// <summary>
    /// Сумма перевода
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Валюта 
    /// </summary>
    public string Currency { get; set; } = null!;

    /// <summary>
    /// Тип транзакции 
    /// </summary>
    public string TransactionType { get; set; } = null!;

    /// <summary>
    /// Описание перевода
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// Время и дата перевода
    /// </summary>
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// Флаг, удалена ли транзакция
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Версия строки (для оптимистической блокировки)
    /// </summary>
    // ReSharper disable once IdentifierTypo
    public uint Xmin { get; set; }
}