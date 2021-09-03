using DotNetCore.CAP;
using DotNetCore.CAP.Internal;
using Ecommerce.Poc.Catalog.Infrastructure.CapFilters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Ecommerce.Poc.Catalog.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static CapBuilder AddCapConsumer(this IServiceCollection services, IConfiguration configuration, string groupName)
        {
            return services.AddCap(x =>
                {
                    x.UseEntityFramework<CatalogDbContext>();

                    x.DefaultGroupName = groupName;

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
                })
                .AddSubscribeFilter<LoggingFilter>();
        }
    }
}