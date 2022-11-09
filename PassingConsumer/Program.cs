using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using PassingConsumer.JobReceivedManager;

namespace PassingConsumer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (string.IsNullOrWhiteSpace(environment))
            {
                environment = "Development";
            }

            var host = new HostBuilder()
                .UseConsoleLifetime()
                .UseEnvironment(environment)
                .ConfigureAppConfiguration(builder =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"), optional: false, reloadOnChange: false);
                })
                .ConfigureServices((context, services) =>
                {
                    services
                    .AddScoped<JobSuccessfullyManager>()
                    .AddHostedService<ListenerService>()
                    .TryAddSingleton(services);
                })
                .Build();

            await host.RunAsync();
        }
    }
}