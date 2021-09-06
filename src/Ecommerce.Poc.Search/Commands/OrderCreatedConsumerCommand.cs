using System;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using Ecommerce.Poc.Search.Consumers;
using Ecommerce.Poc.Search.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Poc.Search.Commands
{
    [Command("order-created-consumer")]
    public class OrderCreatedConsumerCommand : ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            await CreateConsumerHost(Array.Empty<string>(), Program.Configuration).Build().RunAsync();
        }
        
        private static IHostBuilder CreateConsumerHost(string[] args, IConfiguration configuration) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(builder =>
                {
                    builder.AddSimpleConsole(options =>
                    {
                        options.IncludeScopes = true;
                    });
                })
                .ConfigureServices(services =>
                {
                    services
                        .AddElasticClient(configuration)
                        .AddScoped<OrderCreatedConsumer>()
                        .AddCapConsumer(configuration, "order_created");
                });
    }
}