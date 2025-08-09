using System.Net.Http.Json;
using System.Text.Json;
using AccountService.Application.DTOs;
using AccountService.Application.Features.Transactions.MakeTransactions;
using AccountService.Requests;
using AccountService.Responses;
using Xunit;
using Xunit.Abstractions;

namespace AccountService.Tests.IntegrationTests;

public class ParallelTransferTests(IntegrationTestWebFactory factory, ITestOutputHelper output) : BaseIntegrationTest(factory, output)
{
	[Fact]
	public async Task Transfer_Parallel_Accounts() 
	{
		var ownerId1 = Guid.NewGuid();
		var ownerId2 = Guid.NewGuid();
		var createAccount1 = new CreateAccountRequest("USD", "Checking", null);
		var createAccount2 = new CreateAccountRequest("USD", "Checking", null);
		
		var account1 = await Client.PostAsJsonAsync("api/accounts", createAccount1);
		account1.Headers.Add(AuthTestHandler.HUserId, ownerId1.ToString());
		
		var account2 = await Client.PostAsJsonAsync("api/accounts", createAccount2);
		account2.Headers.Add(AuthTestHandler.HUserId, ownerId2.ToString());
		
		// Создание дву счетов
		var accountResponse1 = await account1.Content.ReadFromJsonAsync<MbResult<Guid>>(JsonSerializerOptions);
		var accountResponse2 = await account2.Content.ReadFromJsonAsync<MbResult<Guid>>(JsonSerializerOptions);
		
		Output.WriteLine(accountResponse1!.Result.ToString());
		Output.WriteLine(accountResponse2!.Result.ToString());
		
		// Проверка успешности создания счетов
		Assert.True(accountResponse1.IsSuccess);
		Assert.True(accountResponse2.IsSuccess);

		var transaction1 = new MakeTransactionCommand(accountResponse1.Result, null, "Credit", "USD", 100, "Test");
		var transaction2 = transaction1 with { AccountId = accountResponse2.Result };
		
		var credit1 = await Client.PostAsJsonAsync("api/transactions", transaction1);
		var credit2 = await Client.PostAsJsonAsync("api/transactions", transaction2);
		
		Output.WriteLine(await credit1.Content.ReadAsStringAsync());
		Output.WriteLine(await credit2.Content.ReadAsStringAsync());
		
		Assert.True(credit1.IsSuccessStatusCode);
		Assert.True(credit2.IsSuccessStatusCode);

		var transfers = Enumerable.Range(0, 50)
			.Select(n => {
				var transactionDto = new MakeTransactionCommand(
					n % 2 == 0 ? accountResponse1.Result : accountResponse2.Result,
					n % 2 == 1 ? accountResponse1.Result : accountResponse2.Result,
					"Debit",
					"USD",
					1,
					"Parallel transfer " + n);
				return Client.PostAsJsonAsync("api/transactions", transactionDto);
			});
		
		var result = await Task.WhenAll(transfers);
		foreach (var res in result)
			Output.WriteLine(await res.Content.ReadAsStringAsync());
		// Assert.All(result.Select(x => x.IsSuccessStatusCode), Assert.True);
		
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
		Assert.Equal(100, updatedAccountRes1.Result.First().Balance);
		Assert.Equal(100, updatedAccountRes2.Result.First().Balance);
	}
}