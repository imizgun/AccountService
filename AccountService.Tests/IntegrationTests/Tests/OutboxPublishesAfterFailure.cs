using System.Net.Http.Json;
using AccountService.Application.Features.Accounts.Operations.CreateAccount;
using AccountService.Application.Shared.DatabaseAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace AccountService.Tests.IntegrationTests.Tests;

[Collection("Sequential")]
public class OutboxPublishesAfterFailure(RabbitIntegrationTestFactory factory, ITestOutputHelper output)
    : BaseIntegrationTest<RabbitIntegrationTestFactory>(factory, output), IClassFixture<RabbitIntegrationTestFactory>
{
    [Fact]
    public async Task OutboxPublishesAfterFailure_ShouldPublishEvent()
    {
        var ownerId = Guid.NewGuid();
        Client.DefaultRequestHeaders.Remove(AuthTestHandler.HUserId);
        Client.DefaultRequestHeaders.Add(AuthTestHandler.HUserId, ownerId.ToString());

        await Factory.StopBrokerAsync();
        var create = await Client.PostAsJsonAsync("api/accounts", new CreateAccountRequest("USD", "Checking", null));
        create.EnsureSuccessStatusCode();

        Output.WriteLine(await create.Content.ReadAsStringAsync());

        using var scope = Factory.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AccountServiceDbContext>();
        var pending = await db.OutboxMessages.CountAsync(x => x.PublishedAt == null);
        Assert.True(pending > 0);

        await Factory.StartBrokerAsync();

        // Ждем, пока запустится брокер и сообщения обработаются
        await Task.Delay(TimeSpan.FromSeconds(10));

        var published = await db.OutboxMessages.CountAsync(x => x.PublishedAt != null);
        Assert.True(published > 0);
    }
}