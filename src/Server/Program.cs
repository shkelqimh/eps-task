using Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Server.Config;

namespace Server;

class Program
{
    public static async Task Main()
    {
        var provider = ServiceLocator.ConfigureServices();

        var settings = provider.GetRequiredService<IOptions<Settings>>().Value;
        _ = new SetupDb(settings.ConnectionString);

        var server = new DiscountServer(provider);
        await server.StartAsync();
    }
}