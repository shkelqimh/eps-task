using Infrastructure.Persistence.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Server.Config;

namespace Server;

public static class ServiceLocator
{
    private static ServiceProvider _serviceProvider;

    public static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var settings = config.GetSection("Settings");
        var connectionString = settings.GetValue<string>("ConnectionString");
        
        services.Configure<Settings>(settings);
        services.AddSingleton<ICodeRepository, CodeRepository>(
            _ => new CodeRepository(new SqliteConnection(connectionString)));

        _serviceProvider = services.BuildServiceProvider();

        return _serviceProvider;
    }

    public static T GetService<T>() where T : notnull
    {
        return _serviceProvider.GetRequiredService<T>();
    }
}