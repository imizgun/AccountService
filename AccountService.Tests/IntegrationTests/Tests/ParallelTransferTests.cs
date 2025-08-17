using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AccountService.Application.Features.Accounts;
using AccountService.Application.Features.Accounts.Operations.CreateAccount;
using AccountService.Application.Features.Transactions.Operations.MakeTransactions;
using AccountService.Application.Shared.Contracts;
using Xunit;
using Xunit.Abstractions;

namespace AccountService.Tests.IntegrationTests.Tests;

[Collection("Sequential")]
public class ParallelTransferTests(IntegrationTestWebFactory factory, ITestOutputHelper output)
    : BaseIntegrationTest<IntegrationTestWebFactory>(factory, output), IClassFixture<IntegrationTestWebFactory>
{
    [Fact]
    public async Task Transfer_Parallel_Accounts()
    {
        var ownerId1 = Guid.NewGuid();
        var ownerId2 = Guid.NewGuid();
        const decimal balance = 100m;
        var createAccount1 = new CreateAccountRequest("USD", "Checking", null);
        var createAccount2 = new CreateAccountRequest("USD", "Checking", null);

        var acc1Res = new HttpRequestMessage(HttpMethod.Post, "api/accounts");
        acc1Res.Content = new StringContent(JsonSerializer.Serialize(createAccount1), Encoding.UTF8, "application/json");
        acc1Res.Headers.Add(AuthTestHandler.HUserId, ownerId1.ToString());

        var account1 = await Client.SendAsync(acc1Res);

        var acc2Res = new HttpRequestMessage(HttpMethod.Post, "api/accounts");
        acc2Res.Content = new StringContent(JsonSerializer.Serialize(createAccount2), Encoding.UTF8, "application/json");
        acc2Res.Headers.Add(AuthTestHandler.HUserId, ownerId2.ToString());

        var account2 = await Client.SendAsync(acc2Res);

        // Создание дву счетов
        var accountResponse1 = await account1.Content.ReadFromJsonAsync<MbResult<Guid>>(JsonSerializerOptions);
        var accountResponse2 = await account2.Content.ReadFromJsonAsync<MbResult<Guid>>(JsonSerializerOptions);

        // Проверка успешности создания счетов
        Assert.True(accountResponse1!.IsSuccess);
        Assert.True(accountResponse2!.IsSuccess);

        var transaction1 = new MakeTransactionRequest(accountResponse1.Result, null, "Credit", "USD", balance, "Test");
        var transaction2 = transaction1 with { AccountId = accountResponse2.Result };

        var credit1 = await Client.PostAsJsonAsync("api/transactions", transaction1);
        var credit2 = await Client.PostAsJsonAsync("api/transactions", transaction2);

        Output.WriteLine(await credit1.Content.ReadAsStringAsync());
        Output.WriteLine(await credit2.Content.ReadAsStringAsync());

        Assert.True(credit1.IsSuccessStatusCode);
        Assert.True(credit2.IsSuccessStatusCode);

        var transfers = Enumerable.Range(0, 50)
            .Select(n =>
            {
                var transactionDto = new MakeTransactionRequest(
                    n % 2 == 0 ? accountResponse1.Result : accountResponse2.Result,
                    n % 2 == 1 ? accountResponse1.Result : accountResponse2.Result,
                    "Debit",
                    "USD",
                    1,
                    "Parallel transfer " + n);
                return Client.PostAsJsonAsync("api/transactions", transactionDto);
            });

        var results = await Task.WhenAll(transfers);
        foreach (var transfer in results) Output.WriteLine(transfer.StatusCode.ToString());

        var r1 = new HttpRequestMessage(HttpMethod.Get, "api/accounts");
        r1.Headers.Add(AuthTestHandler.HUserId, ownerId1.ToString());
        using var updatedAccount1 = await Client.SendAsync(r1);

        var r2 = new HttpRequestMessage(HttpMethod.Get, "api/accounts");
        r2.Headers.Add(AuthTestHandler.HUserId, ownerId2.ToString());
        using var updatedAccount2 = await Client.SendAsync(r2);

        var accText1 = await updatedAccount1.Content.ReadAsStringAsync();
        var updatedAccountRes1 = JsonSerializer.Deserialize<MbResult<List<AccountDto>>>(accText1, JsonSerializerOptions);

        var accText2 = await updatedAccount2.Content.ReadAsStringAsync();
        var updatedAccountRes2 = JsonSerializer.Deserialize<MbResult<List<AccountDto>>>(accText2, JsonSerializerOptions);

        Output.WriteLine(accText1);
        Output.WriteLine(accText2);

        Assert.NotNull(updatedAccountRes1);
        Assert.NotNull(updatedAccountRes2);
        Assert.NotNull(updatedAccountRes1.Result);
        Assert.NotNull(updatedAccountRes2.Result);
        Assert.True(results.Any(x => x.IsSuccessStatusCode), "Not all transfers were successful");
        Assert.Equal(2 * balance, updatedAccountRes1.Result.First().Balance + updatedAccountRes2.Result.First().Balance);
    }
}