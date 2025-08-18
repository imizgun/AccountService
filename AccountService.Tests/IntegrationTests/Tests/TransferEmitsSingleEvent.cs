using System.Net.Http.Json;
using System.Text.Json;
using AccountService.Application.Features.Accounts.Operations.CreateAccount;
using AccountService.Application.Features.Transactions.Events;
using AccountService.Application.Features.Transactions.Operations.MakeTransactions;
using AccountService.Application.Shared.Contracts;
using AccountService.Application.Shared.DatabaseAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace AccountService.Tests.IntegrationTests.Tests;


[Collection("Sequential")]
public class TransferEmitsSingleEvent(RabbitIntegrationTestFactory factory, ITestOutputHelper output) :
    BaseIntegrationTest<RabbitIntegrationTestFactory>(factory, output), IClassFixture<RabbitIntegrationTestFactory>
{
    [Fact]
    public async Task TransferEmitsSingleEvent_ShouldEmitSingleEvent()
    {
        var ownerId = Guid.NewGuid();
        Client.DefaultRequestHeaders.Remove(AuthTestHandler.HUserId);
        Client.DefaultRequestHeaders.Add(AuthTestHandler.HUserId, ownerId.ToString());

        await Factory.StopBrokerAsync();
        var create = await Client.PostAsJsonAsync("api/accounts", new CreateAccountRequest("USD", "Checking", null));
        create.EnsureSuccessStatusCode();

        var str = await create.Content.ReadAsStringAsync();
        var account = JsonSerializer.Deserialize<MbResult<Guid>>(str, JsonSerializerOptions);

        Output.WriteLine(str);

        Assert.NotNull(account);
        Assert.NotEqual(Guid.Empty, account.Result);

        var transferRequest = new MakeTransactionRequest(
            account.Result,
            null,
            "Credit",
            "USD",
            100,
            "Test transfer");

        for (var i = 0; i < 50; i++)
            await Client.PostAsJsonAsync("api/transactions", transferRequest);

        using var scope = Factory.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AccountServiceDbContext>();

        Assert.Equal(50, await ctx.OutboxMessages.Where(x => x.Type.Contains(nameof(TransferCompleted))).CountAsync());
    }
}