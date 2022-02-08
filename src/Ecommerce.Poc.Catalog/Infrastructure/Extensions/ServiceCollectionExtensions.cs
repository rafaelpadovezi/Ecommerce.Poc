using DotNetCore.CAP;
using DotNetCore.CAP.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Ziggurat.CapAdapter;

namespace Ecommerce.Poc.Catalog.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static CapBuilder AddCatalogCap(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddCap(x =>
                {
                    x.UseEntityFramework<CatalogDbContext>();

                    x.DefaultGroupName = "catalog";

                    x.UseRabbitMQ(o =>
                    {
                        o.HostName = configuration.GetValue<string>("RabbitMQ:HostName");
                        o.Port = configuration.GetValue<int>("RabbitMQ:Port");
                        o.ExchangeName = configuration.GetValue<string>("RabbitMQ:ExchangeName");
                        // To consume messages without the cap headers - https://cap.dotnetcore.xyz/user-guide/en/cap/messaging/#custom-headers
                        o.CustomHeaders = e => new List<KeyValuePair<string, string>>
                        {
                            new(DotNetCore.CAP.Messages.Headers.MessageId, SnowflakeId.Default().NextId().ToString()),
                            new(DotNetCore.CAP.Messages.Headers.MessageName, e.RoutingKey)
                        };
                    });
                })
                .AddSubscribeFilter<BootstrapFilter>();
        }
    }
}