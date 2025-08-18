using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AccountService.Application.Features.Accounts;
using AccountService.Application.Features.Accounts.Events;
using AccountService.Application.Features.Accounts.Operations.CreateAccount;
using AccountService.Application.Features.Transactions.Operations.MakeTransactions;
using AccountService.Application.Shared.Contracts;
using AccountService.Application.Shared.DatabaseAccess;
using AccountService.Application.Shared.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace AccountService.Tests.IntegrationTests.Tests;

[Collection("Sequential")]
public class ClientBlockedPreventsDebit(RabbitIntegrationTestFactory factory, ITestOutputHelper output)
    : BaseIntegrationTest<RabbitIntegrationTestFactory>(factory, output), IClassFixture<RabbitIntegrationTestFactory>
{
    [Fact]
    public async Task ClientBlockedEventPreventsDebit_ShouldReturnConflict()
    {
        var ownerId1 = Guid.NewGuid();
        const decimal balance = 100m;

        // Создание счета для клиента
        var createAccount1 = new CreateAccountRequest("USD", "Checking", null);

        var acc1Res = new HttpRequestMessage(HttpMethod.Post, "api/accounts");
        acc1Res.Content = new StringContent(JsonSerializer.Serialize(createAccount1), Encoding.UTF8, "application/json");
        acc1Res.Headers.Add(AuthTestHandler.HUserId, ownerId1.ToString());

        var account1 = await Client.SendAsync(acc1Res);

        var res = await account1.Content.ReadAsStringAsync();

        var accountResponse1 = JsonSerializer.Deserialize<MbResult<Guid>>(res, JsonSerializerOptions);

        Assert.True(accountResponse1!.IsSuccess);

        // Начисление средств
        var transaction1 = new MakeTransactionRequest(accountResponse1.Result, null, "Credit", "USD", balance, "Test");

        var credit1 = await Client.PostAsJsonAsync("api/transactions", transaction1);
        Output.WriteLine(await credit1.Content.ReadAsStringAsync());

        Assert.True(credit1.IsSuccessStatusCode);


        // Блокировка клиента
        using var scope = Factory.Services.CreateScope();

        var publish = scope.ServiceProvider.GetRequiredService<IBus>();
        var @event = new ClientBlocked(
            eventId: Guid.NewGuid(),
            occurredAt: DateTime.UtcNow,
            meta: new Meta(Guid.NewGuid()),
            clientId: ownerId1
        );

        Assert.Equal("v1", @event.Meta.Version);
        Assert.NotEqual(Guid.Empty, @event.EventId);

        await publish.Publish(@event, ctx =>
        {
            ctx.SetRoutingKey("client.blocked");
            ctx.MessageId = @event.EventId;
        });


        await RabbitIntegrationTestFactory.WaitUntilAsync(async () =>
        {
            using var s = Factory.Services.CreateScope();
            var db = s.ServiceProvider.GetRequiredService<AccountServiceDbContext>();
            return await db.InboxConsumed.AnyAsync(x =>
                x.MessageId == @event.EventId);
        }, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(100));

        var dbContext = scope.ServiceProvider.GetRequiredService<AccountServiceDbContext>();
        Output.WriteLine((await dbContext.InboxConsumed.CountAsync()).ToString());
        Output.WriteLine((await dbContext.InboxDeadLetters.CountAsync()).ToString());

        var transaction2 = new MakeTransactionRequest(accountResponse1.Result, null, "Debit", "USD", balance / 2, "Test");

        var debit = await Client.PostAsJsonAsync("api/transactions", transaction2);

        Output.WriteLine(await debit.Content.ReadAsStringAsync());

        Assert.False(debit.IsSuccessStatusCode);

        var r1 = new HttpRequestMessage(HttpMethod.Get, "api/accounts");
        r1.Headers.Add(AuthTestHandler.HUserId, ownerId1.ToString());
        using var updatedAccount1 = await Client.SendAsync(r1);

        var accText1 = await updatedAccount1.Content.ReadAsStringAsync();
        var updatedAccountRes1 = JsonSerializer.Deserialize<MbResult<List<AccountDto>>>(accText1, JsonSerializerOptions);

        Output.WriteLine(accText1);

        Assert.NotNull(updatedAccountRes1);
        Assert.NotNull(updatedAccountRes1.Result);
        Assert.Equal(balance, updatedAccountRes1.Result.First().Balance);
        Assert.True(updatedAccountRes1.Result.First().IsFrozen);
    }

    [Fact]
    public async Task ClientUnblockedEvent_ShouldAllowToMakeTransactions()
    {
        var ownerId1 = Guid.NewGuid();
        const decimal balance = 100m;

        // Создание счета для клиента
        var createAccount1 = new CreateAccountRequest("USD", "Checking", null);

        var acc1Res = new HttpRequestMessage(HttpMethod.Post, "api/accounts");
        acc1Res.Content = new StringContent(JsonSerializer.Serialize(createAccount1), Encoding.UTF8, "application/json");
        acc1Res.Headers.Add(AuthTestHandler.HUserId, ownerId1.ToString());

        var account1 = await Client.SendAsync(acc1Res);

        var res = await account1.Content.ReadAsStringAsync();

        var accountResponse1 = JsonSerializer.Deserialize<MbResult<Guid>>(res, JsonSerializerOptions);

        Assert.True(accountResponse1!.IsSuccess);

        // Начисление средств
        var transaction1 = new MakeTransactionRequest(accountResponse1.Result, null, "Credit", "USD", balance, "Test");

        var credit1 = await Client.PostAsJsonAsync("api/transactions", transaction1);
        Output.WriteLine(await credit1.Content.ReadAsStringAsync());

        Assert.True(credit1.IsSuccessStatusCode);


        // Блокировка клиента
        using var scope = Factory.Services.CreateScope();

        var publish = scope.ServiceProvider.GetRequiredService<IBus>();
        var @event = new ClientBlocked(
            eventId: Guid.NewGuid(),
            occurredAt: DateTime.UtcNow,
            meta: new Meta(Guid.NewGuid()),
            clientId: ownerId1
        );

        Assert.Equal("v1", @event.Meta.Version);
        Assert.NotEqual(Guid.Empty, @event.EventId);

        await publish.Publish(@event, ctx =>
        {
            ctx.SetRoutingKey("client.blocked");
            ctx.MessageId = @event.EventId;
        });

        await RabbitIntegrationTestFactory.WaitUntilAsync(async () =>
        {
            using var s = Factory.Services.CreateScope();
            var db = s.ServiceProvider.GetRequiredService<AccountServiceDbContext>();
            return await db.InboxConsumed.AnyAsync(x =>
                x.MessageId == @event.EventId);
        }, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(100));

        var dbContext = scope.ServiceProvider.GetRequiredService<AccountServiceDbContext>();
        Output.WriteLine((await dbContext.InboxConsumed.CountAsync()).ToString());
        Output.WriteLine((await dbContext.InboxDeadLetters.CountAsync()).ToString());

        var transaction2 = new MakeTransactionRequest(accountResponse1.Result, null, "Debit", "USD", balance / 2, "Test");

        var debit = await Client.PostAsJsonAsync("api/transactions", transaction2);

        Output.WriteLine(await debit.Content.ReadAsStringAsync());

        Assert.False(debit.IsSuccessStatusCode);

        var eventUnblock = new ClientUnblocked(
            eventId: Guid.NewGuid(),
            occurredAt: DateTime.UtcNow,
            meta: new Meta(Guid.NewGuid()),
            clientId: ownerId1
        );

        await publish.Publish(eventUnblock, ctx =>
        {
            ctx.SetRoutingKey("client.unblocked");
            ctx.MessageId = eventUnblock.EventId;
        });

        await RabbitIntegrationTestFactory.WaitUntilAsync(async () =>
        {
            using var s = Factory.Services.CreateScope();
            var db = s.ServiceProvider.GetRequiredService<AccountServiceDbContext>();
            return await db.InboxConsumed.AnyAsync(x =>
                x.MessageId == eventUnblock.EventId);
        }, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(100));

        var debit2 = await Client.PostAsJsonAsync("api/transactions", transaction2);
        var res0 = await debit2.Content.ReadAsStringAsync();
        Output.WriteLine(res0);
        Assert.True(debit2.IsSuccessStatusCode);
    }
}