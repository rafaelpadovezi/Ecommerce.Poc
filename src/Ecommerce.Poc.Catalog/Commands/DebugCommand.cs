using System;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using Ecommerce.Poc.Catalog.Consumers;
using Ecommerce.Poc.Catalog.Domain.Services;
using Ecommerce.Poc.Catalog.Dtos;
using Ecommerce.Poc.Catalog.Infrastructure;
using Ecommerce.Poc.Catalog.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ziggurat;

namespace Ecommerce.Poc.Catalog.Commands
{
    /// <summary>
    /// Start all consumers and the API
    /// </summary>
    [Command("debug")]
    public class RunAllCommand : ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console) =>
            await Program.CreateHostBuilder(Array.Empty<string>())
                .ConfigureServices(services =>
                {
                    services.AddScoped<OrderCanceledConsumer>();
                    services
                        .AddScoped<OrderCreatedConsumer>()
                        .AddConsumerService<OrderCreatedMessage, OrderCreatedService>(
                            options => options.UseIdempotency<CatalogDbContext>());
                    services.AddCatalogCap(Program.Configuration);
                })
                .Build()
                .RunAsync();
    }
}