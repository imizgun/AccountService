using System.Text.Json;
using Xunit.Abstractions;

namespace AccountService.Tests.IntegrationTests;

public abstract class BaseIntegrationTest<TFactory>(TFactory factory, ITestOutputHelper output)
    where TFactory : IntegrationTestWebFactory
{
    protected readonly TFactory Factory = factory;
    protected readonly HttpClient Client = factory.CreateClient();
    protected readonly ITestOutputHelper Output = output;

    protected readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
}