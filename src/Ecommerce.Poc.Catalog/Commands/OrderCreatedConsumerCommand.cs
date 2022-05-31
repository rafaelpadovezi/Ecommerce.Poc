using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Ecommerce.Poc.Catalog.Consumers;
using Ecommerce.Poc.Catalog.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Ecommerce.Poc.Catalog.Domain.Services;
using Ecommerce.Poc.Catalog.Dtos;
using Ecommerce.Poc.Catalog.Infrastructure.ConsumerMiddlewares;
using Ecommerce.Poc.Catalog.Infrastructure.Extensions;
using Ziggurat;

namespace Ecommerce.Poc.Catalog.Commands
{
    [Command("order-created-consumer")]
    public class OrderCreatedConsumerCommand : ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            await CreateConsumerHost(Array.Empty<string>(), Program.Configuration).Build().RunAsync();
        }

        public static IHostBuilder CreateConsumerHost(string[] args, IConfiguration configuration) =>
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
                        .AddScoped<OrderCreatedConsumer>()
                        .AddConsumerService<OrderCreatedMessage, OrderCreatedService>(
                            options =>
                            {
                                options.Use<ValidationMiddleware<OrderCreatedMessage>>();
                                options.UseIdempotency<CatalogDbContext>();
                            })
                        .AddCatalogCap(configuration);

                    services.AddDbContext<CatalogDbContext>(options =>
                        options.UseSqlServer(configuration.GetConnectionString("CatalogDbContext")));
                });
    }
}