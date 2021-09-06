using System;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Ecommerce.Poc.Search.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;

namespace Ecommerce.Poc.Search.Consumers
{
    public class ProductCreatedConsumer : ICapSubscribe
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductCreatedConsumer> _logger;

        public ProductCreatedConsumer(
            IConfiguration configuration,
            ILogger<ProductCreatedConsumer> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [CapSubscribe("product.created")]
        public async Task AddProductAsync(ProductCreatedMessage message)
        {
            var elasticUrl = _configuration.GetValue<string>("ElasticSearch:Url");
            var indexName = _configuration.GetValue<string>("ElasticSearch:IndexName");
            var settings = new ConnectionSettings(new Uri(elasticUrl))
                .DefaultIndex(indexName)
                .EnableDebugMode();

            var client = new ElasticClient(settings);
            
            var response = await client.IndexDocumentAsync(message);
            if (response.IsValid)
                _logger.LogInformation("Product added.");
            else
                _logger.LogWarning($"Product not added: {response.ServerError}");
        }
    }
}