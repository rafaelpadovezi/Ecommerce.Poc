using System;
using System.Collections.Generic;
using System.Reflection;
using DotNetCore.CAP;
using DotNetCore.CAP.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

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

                x.DefaultGroupName = $"search_{groupName}";

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

        public static IServiceCollection AddElasticClient(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var elasticUrl = configuration.GetValue<string>("ElasticSearch:Url");
            var indexName = configuration.GetValue<string>("ElasticSearch:IndexName");
            var settings = new ConnectionSettings(new Uri(elasticUrl))
                .DefaultIndex(indexName)
                .EnableDebugMode();

            var client = new ElasticClient(settings);
            // https://www.elastic.co/guide/en/elasticsearch/client/net-api/master/lifetimes.html
            services.AddSingleton<IElasticClient>(_ => client);
            return services;
        }
    }
}