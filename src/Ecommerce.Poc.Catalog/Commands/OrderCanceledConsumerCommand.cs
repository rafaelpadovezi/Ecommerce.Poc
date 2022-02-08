using CliFx;
using CliFx.Attributes;
using Ecommerce.Poc.Catalog.Consumers;
using Ecommerce.Poc.Catalog.Infrastructure;
using Ecommerce.Poc.Catalog.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Ecommerce.Poc.Catalog.Commands
{
    [Command("order-canceled-consumer")]
    public class OrderCanceledConsumerCommand : ICommand
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
                        .AddScoped<OrderCanceledConsumer>()
                        .AddCatalogCap(configuration);

                    services.AddDbContext<CatalogDbContext>(options =>
                        options.UseSqlServer(configuration.GetConnectionString("CatalogDbContext")));
                });
    }
}