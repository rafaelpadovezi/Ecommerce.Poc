using System.Collections.Generic;
using DotNetCore.CAP;
using DotNetCore.CAP.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Poc.Search.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static CapBuilder AddCapConsumer(this IServiceCollection services, IConfiguration configuration, string groupName)
        {
            return services.AddCap(x =>
            {
                // CAP needs a storage to work
                x.UseInMemoryStorage();

                x.DefaultGroupName = groupName;

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
            });
        }
    }
}