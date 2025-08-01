# Account Service
## Сервис для обслуживания процессов розничного банка

## Использованные технологии
- ASP.NET Core
- MediatR
- FluentValidation
- AutoMapper

## Структура проекта
- `AccountService` (Контроллеры и `Program.cs`)
- `AccountService.Application` (MediatR-компоненты, сервисы заглушки для проверки юзеров и валют)
- `AccountService.Core` (доменные логика и сущности)
- `AccountService.DatabaseAccess` (заглушка для эмуляции обращение к БД)

## Доступные эндпоинты
- `GET /api/clients` - возвращает список из `Guid` доступных юзеров. \
Пример ответа:
```
[
  "307e28c7-82d5-4ace-9489-d4f1f39451d0",
  "f3d4c11a-ea60-4727-a06b-e35d66a6fe89",
  "d7bd952b-ecfb-4284-8536-84392c500995"
]
```

- `GET /api/accounts?ownerId={id}` - возвращает список открытых у юзера `id` счетов.\
Пример ответа:
```
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "ownerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "accountType": "string",
    "currency": "string",
    "balance": 0,
    "interestRate": 0,
    "openingDate": "2025-08-01T21:13:51.887Z",
    "closingDate": "2025-08-01T21:13:51.887Z"
  }
]
```

- `POST /api/accounts` - создает счет. Пример тела запроса:
```
{
  "ownerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "currency": "string",
  "accountType": "string",
  "interestRate": 0
}
```
`accountType` должен быть валидной строкой, `currency` корректной валютой.

Ответ:
```
{
  "instanceId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

- `DELETE /api/accounts/{id}` - удаляет (закрывает) счет по ID `id`.
Пример ответа:
```
{
  "message": "string"
}
```

- `PATCH /api/accounts/{id}` - изменяет процентную ставку по счету, если тип счета соответствующий. Пример тела:
```
{
  "interestRate": 0
}
```
Ответ:
```
{
  "message": "string"
}
```

- `GET /api/accounts/{id}/transactions?skip=x&take=y` - получение выписки по счету `id`, пропустив `skip` записей, взяв `take` записей.
Пример ответа:
```
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "accountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "counterpartyAccountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "amount": 0,
    "currency": "string",
    "transactionType": "string",
    "description": "string",
    "transactionDate": "2025-08-01T21:25:09.688Z",
    "isDeleted": true
  }
]
```

- `POST /api/accounts/{id}/transactions` - создает транзакцию для счета `id`. Пример тела:
```
{
  "counterpartyAccountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "transactionType": "string",
  "currency": "string",
  "amount": 0,
  "description": "string"
}
```

Ответ - `id` созданной транзакции.
```
{
  "instanceId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

- `DELETE /api/accounts/{id}/transactions/{transactionId}` - помечает транзакцию `transactionId` как удаленную  для счета `id`. Пример ответа:
```
{
  "message": "string"
}
```

- `PATCH /api/accounts/{id}/transactions/{transactionId}` - изменяет у транзакции `transactionId` описание для счета `id`. Пример тела:

```
{
  "description": "string"
}
```
Пример ответа:
```
{
  "message": "string"
}
```