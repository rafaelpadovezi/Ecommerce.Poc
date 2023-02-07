using DotNetCore.CAP;
using DotNetCore.CAP.Internal;
using DotNetCore.CAP.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using Ziggurat.CapAdapter;

namespace Ecommerce.Poc.Catalog.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOtel(this IServiceCollection services, string serviceName)
        {
            return services.AddOpenTelemetryTracing((builder) => builder
                .AddAspNetCoreInstrumentation()
                .AddSqlClientInstrumentation(options => options.SetDbStatementForText = true)
                .SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(serviceName))
                .AddCapInstrumentation()
                .AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(Program.Configuration.GetValue<string>("Otlp:Endpoint"));
                    otlpOptions.Protocol = OtlpExportProtocol.Grpc;
                })
            );
        }
        
        public static CapBuilder AddCatalogCap(this IServiceCollection services, IConfiguration configuration)
        {
            var x = services.AddCap(x =>
                {
                    x.UseEntityFramework<CatalogDbContext>();

                    x.DefaultGroupName = "catalog";
                    x.ConsumerThreadCount = 1;
                    x.EnableConsumerPrefetch = false;

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
            
            var descriptor =
                new ServiceDescriptor(
                    typeof(IConsumerClientFactory),
                    typeof(CustomRabbitMQConsumerClientFactory),
                    ServiceLifetime.Singleton);
            services.Replace(descriptor);

            return x;
        }
    }
}