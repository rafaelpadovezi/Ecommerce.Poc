using System;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Ecommerce.Poc.Search.Consumers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Ecommerce.Poc.Search.Commands
{
    /// <summary>
    /// Start all consumers and the API
    /// </summary>
    [Command("debug")]
    public class RunAllCommand : ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console) =>
            await ApiCommand.CreateHostBuilder(Array.Empty<string>())
                .ConfigureServices(services =>
                {
                    services.AddScoped<ProductCreatedConsumer>();
                    services.AddScoped<OrderCreatedConsumer>();
                })
                .Build()
                .RunAsync();
    }
}