using System.Text.Json;
using Xunit;
using Xunit.Abstractions;

namespace AccountService.Tests.IntegrationTests;

public class BaseIntegrationTest(IntegrationTestWebFactory factory, ITestOutputHelper output) : IClassFixture<IntegrationTestWebFactory>
{
	protected readonly HttpClient Client = factory.CreateClient();
	protected readonly ITestOutputHelper Output = output;
	protected readonly JsonSerializerOptions JsonSerializerOptions = new()
	{
		PropertyNameCaseInsensitive = true,
	};
}