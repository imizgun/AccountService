# Account Service
## Сервис для обслуживания процессов розничного банка

## Использованные технологии
- ASP.NET Core
- MediatR
- FluentValidation
- AutoMapper
- Hangfire
- PostgreSQL

## Структура проекта
- `AccountService` (Контроллеры и `Program.cs`)
- `AccountService.Application` (MediatR-компоненты, сервисы заглушки для проверки юзеров и валют)
- `AccountService.Core` (доменные логика и сущности)
- `AccountService.DatabaseAccess` (Работа БД)
- `AccountService.Background` (Background задачи, в данном случае - начисление процентов по счетам)
- `AccountService.Tests` (Тесты)

## Доступные эндпоинты
## 1. Все ответы API формата `MbResult`:
```
{
  "isSuccess": true,            // флаг успешности операции
  "result": operationResult,    // тело ответа
  "message": "string",          // сообщение
  "mbError": {                  // mbError с более подробным описание
    "message": "string",        // сообщение ошибки
    "validationErrors": [
      "string"                  // массив ошибок
    ]
  }
}
```
### Все эндпоинты требуют авторизации через Keycloak, для этого нужно получить токен доступа и указать его
- `GET /api/accounts` - возвращает список открытых у юзера счетов.

- `POST /api/accounts` - создает счет. Пример тела запроса:
```
{
  "currency": "string",
  "accountType": "string",
  "interestRate": 0         // процентная ставка, если счет Deposit или Credit
}
```
`accountType` должен быть валидной строкой, `currency` корректной валютой.

- `DELETE /api/accounts/{id}` - удаляет (закрывает) счет по ID `id`.

- `PATCH /api/accounts/{id}` - изменяет процентную ставку по счету, если тип счета соответствующий. Пример тела:
```
{
  "interestRate": 0
}
```
- `GET /api/accounts/{id}/transactions?skip=x&take=y` - получение выписки по счету `id`, пропустив `skip` записей, взяв `take` записей.

- `POST /api/transactions` - создает транзакцию для счета `id`. Пример тела:
```
{
  "accountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "counterpartyAccountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "transactionType": "string",
  "currency": "string",
  "amount": 0,
  "description": "string"
}
```

- `DELETE /api/transactions/{transactionId}` - помечает транзакцию `transactionId` как удаленную. Пример ответа:


- `PATCH /api/transactions/{transactionId}` - изменяет у транзакции `transactionId` описание. Пример тела:

```
{
  "description": "string"
}
```

---
## 2. Два зарегистрированных юзера в Keycloak
Keycloak используется как сервер авторизации, доступны два юзера:
```
username: violetta
password: 123
```

```
username: johnprok
password: 123
```

другие важные поля для Keycloak:
```
{
    client_id: aspnet-api,
    client_secret: 9qKJ6qvOOOnlbg1uZoSbtIIHk7Elaxik,
    grant_type: password
}
```

## 3. Запуск приложения
### 3.1 Через docker-compose
Из корневой папки репозитория введите команду: `docker-compose up --build -d`, после чего немного подождите, прежде чем Keycloak настроится (обычно около 20 секунд).\
Затем из Postman или другого удобного инструмента отправьте запрос на `http://localhost:8888/realms/account_service/protocol/openid-connect/token`
с телом типа `x-www-form-urlenconded`: 
```
{
    client_id: aspnet-api,
    client_secret: 9qKJ6qvOOOnlbg1uZoSbtIIHk7Elaxik,
    grant_type: password,
    username: <выбранный_юзернейм>,
    password: <пароль_юзера>
}
```
после этого скопируйте `accessToken` и вставьте его в `swagger` по адресу `http://localhost:80/swagger` в форму авторизации, чтобы иметь возможность обращаться к API.

### 3.2 Через VS 2022 в контейнере с дебагом
В VS 2022 выберите профиль Docker Compose и запустите его в режиме дебагга (F5)

## 4. Тесты
Тесты можно запустить как через IDE, так и через консоль, для этого нужно ввести команду `dotnet test` в папке `AccountService.Tests`.

## 5. Hangfire
Hangfire доступен по адресу `http://localhost:80/hangfire`, там можно посмотреть запланированные задачи, а также запустить их вручную для проверки.