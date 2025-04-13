using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Server;
using Server.Config;

namespace Server.IntegrationTests;

public class TcpServerFixture : IAsyncLifetime
{
    public DiscountServer Server { get; private set; }
    private CancellationTokenSource _cts;

    public async Task InitializeAsync()
    {
        _cts = new CancellationTokenSource();
        var services = CreateTestServices();
        Server = new DiscountServer(services);
        _ = Task.Run(() => Server.StartAsync(_cts.Token));
        await Task.Delay(500);
    }

    public Task DisposeAsync()
    {
        _cts.Cancel();
        return Task.CompletedTask;
    }

    private IServiceProvider CreateTestServices()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(Options.Create(new Settings
        {
            Port = 50051,
            WhiteListedIpAddresses = new[] { "127.0.0.1" },
            CodeRules = new CodeRules { MaxCodeGenerationWithinRequestAllowed = 100, MinCodeLength = 5, MaxCodeLength = 10 }
        }));

        serviceCollection.AddSingleton<ICodeRepository, CodeRepository>();

        return serviceCollection.BuildServiceProvider();
    }
}