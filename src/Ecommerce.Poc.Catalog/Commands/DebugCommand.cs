using System;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using Ecommerce.Poc.Catalog.Consumers;
using Ecommerce.Poc.Catalog.Infrastructure;
using Ecommerce.Poc.Catalog.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

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
                    services.AddScoped<OrderCreatedConsumer>();
                })
                .Build()
                .RunAsync();
    }
}