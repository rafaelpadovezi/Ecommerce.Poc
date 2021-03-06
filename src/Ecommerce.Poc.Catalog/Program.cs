using CliFx;
using Ecommerce.Poc.Catalog.Commands;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace Ecommerce.Poc.Catalog
{
    public static class Program
    {
        internal static IConfiguration Configuration { get; set; }

        public static async Task<int> Main()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables().Build();
            // Commands are declared on Commands folder
            return await new CliApplicationBuilder()
                .AddCommandsFromThisAssembly()
                .Build()
                .RunAsync();
        }

        // EF tools uses this method to obtain the DbContext. See: https://docs.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli#from-application-services
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<ApiCommand.Startup>();
                });
    }
}
