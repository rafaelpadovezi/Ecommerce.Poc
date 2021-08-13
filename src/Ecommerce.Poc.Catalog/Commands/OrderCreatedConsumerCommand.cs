using CliFx;
using CliFx.Attributes;
using DotNetCore.CAP.Internal;
using Ecommerce.Poc.Catalog.Consumers;
using Ecommerce.Poc.Catalog.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
                        .AddCap(x =>
                        {
                            x.UseEntityFramework<CatalogDbContext>();

                            x.UseRabbitMQ(o =>
                            {
                                o.HostName = Program.Configuration.GetValue<string>("RabbitMQ:HostName");
                                o.Port = Program.Configuration.GetValue<int>("RabbitMQ:Port");
                                o.ExchangeName = Program.Configuration.GetValue<string>("RabbitMQ:ExchangeName");
                                // To consume messages without the cap headers - https://cap.dotnetcore.xyz/user-guide/en/cap/messaging/#custom-headers
                                o.CustomHeaders = e => new List<KeyValuePair<string, string>>
                                {
                                    new(DotNetCore.CAP.Messages.Headers.MessageId, SnowflakeId.Default().NextId().ToString()),
                                    new(DotNetCore.CAP.Messages.Headers.MessageName, e.RoutingKey)
                                };
                            });
                        });
                    services.AddDbContext<CatalogDbContext>(options =>
                        options.UseSqlServer(configuration.GetConnectionString("CatalogDbContext")));
                });
    }
}