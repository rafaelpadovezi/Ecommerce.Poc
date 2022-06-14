using System;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Ecommerce.Poc.Catalog.Consumers;
using Ecommerce.Poc.Catalog.Domain.Services;
using Ecommerce.Poc.Catalog.Dtos;
using Ecommerce.Poc.Catalog.Infrastructure;
using Ecommerce.Poc.Catalog.Infrastructure.ConsumerMiddlewares;
using Ecommerce.Poc.Catalog.Infrastructure.Extensions;
using Ecommerce.Poc.Catalog.Validators;
using FluentValidation;
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
                    // Validation
                    services.AddScoped<IValidatorFactory>(s => new ServiceProviderValidatorFactory(s));
                    services.AddScoped<IValidator<OrderCreatedMessage>, OrderCreatedMessageValidator>();
                    
                    services.AddScoped<OrderCanceledConsumer>();
                    services
                        .AddScoped<OrderCreatedConsumer>()
                        .AddConsumerService<OrderCreatedMessage, OrderCreatedService>(
                            options =>
                            {
                                options.Use<ValidationMiddleware<OrderCreatedMessage>>();
                                options.UseEntityFrameworkIdempotency<OrderCreatedMessage, CatalogDbContext>();
                            });
                    services.AddCatalogCap(Program.Configuration);
                })
                .Build()
                .RunAsync();
    }
}